import { NgIf } from "@angular/common";
import { Component, ComponentRef, Input, OnInit, ViewChild } from "@angular/core";
import { DxButtonModule, DxCheckBoxModule, DxDataGridComponent, DxDataGridModule } from 'devextreme-angular';
import Guid from 'devextreme/core/guid';
import DataSource from "devextreme/data/data_source";
import { ValueChangedEvent } from "devextreme/ui/check_box";
import { Common } from 'src/app/classes';
import { SlidePanelService } from "src/app/layouts/slide-panel/slide-panel.services";
import { Role } from "src/app/data/models";
import { Context } from "src/app/data";
import { Subject, takeUntil } from "rxjs";
import { faLink, faPlus } from "@fortawesome/pro-solid-svg-icons";
import { icon } from "@fortawesome/fontawesome-svg-core";
import { DataEventService } from "src/app/shared/services";
import { GroupCreateComponent } from "../../group/create.component";

@Component({
    selector: 'sh-role-group-link',
    templateUrl: 'link.component.html',
    styleUrls: ['link.component.scss'],
    imports: [NgIf, DxButtonModule, DxDataGridModule, DxCheckBoxModule]
})
export class Role_GroupLinkComponent implements OnInit {
    protected readonly _destroying$ = new Subject<void>();

    @ViewChild('rolesDataGrid', { static: false }) rolesDataGridComponent!: DxDataGridComponent;
    @ViewChild('groupsDataGrid', { static: false }) groupsDataGridComponent!: DxDataGridComponent;

    // Block first change from triggering a reload, but reload directly after
    protected _rolesIsLoading: boolean = true;
    protected _rolesRefreshQueue: number = 0;
    protected _groupsIsLoading: boolean = true;
    protected _groupsRefreshQueue: number = 0;

    @Input() roleId?: Guid;
    @Input() groupId?: Guid;

    get showRoles(): boolean { return Common.isNull(this.roleId); }
    get showGroups(): boolean { return Common.isNull(this.groupId); }

    rolesDataSource: DataSource;
    groupsDataSource: DataSource;

    selectedRoles: Guid[] = [];
    selectedGroups: Guid[] = [];

    fasLink = icon(faLink).html[0];
    fasPlus = icon(faPlus).html[0];

    constructor(
        protected context: Context,
        protected dataEventService: DataEventService,
        protected slideService: SlidePanelService
    ) {
        this.rolesDataSource = new DataSource({
            store: context.roles,
            select: ['id', 'name', 'operation', 'target'],
            onLoadingChanged: this.onRolesLoadingChanged.bind(this)
        });

        this.groupsDataSource = new DataSource({
            store: context.groups,
            select: ['id', 'name'],
            onLoadingChanged: this.onGroupsLoadingChanged.bind(this)
        });

        this.createGroup = this.createGroup.bind(this);
        this.addGroupToRoles = this.addGroupToRoles.bind(this);
        this.addRoleToGroups = this.addRoleToGroups.bind(this);
        this.refreshRoles = this.refreshRoles.bind(this);
        this.refreshGroups = this.refreshGroups.bind(this);
        this.onRoleCheckboxClick = this.onRoleCheckboxClick.bind(this);
        this.onRolesLoadingChanged = this.onRolesLoadingChanged.bind(this);
        this.onGroupsLoadingChanged = this.onGroupsLoadingChanged.bind(this);
    }

    public async ngOnInit(): Promise<void> {
        this.dataEventService.groupChanged$
            .pipe(takeUntil(this._destroying$))
            .subscribe(async () => { await this.refreshGroups(); });

        this.dataEventService.roleChanged$
            .pipe(takeUntil(this._destroying$))
            .subscribe(async () => { await this.refreshRoles(); });
    }

    createGroup(): void {
        this.slideService.pushComponent({
            componentType: GroupCreateComponent,
            setInputs: (componentRef: ComponentRef<GroupCreateComponent>) => {
            },
            title: `Create Group`,
            forceOverlay: true
        });
    }

    async onRoleCheckboxClick(e: ValueChangedEvent, groupData: any) {
        if (e.value) {
            groupData?.data?.items?.forEach((role: Role) => {
                if (Common.isNotNull(role) && Common.isNotNull(role.id)) {
                    if (!this.selectedRoles.some((element: Guid) => {
                        if (element === role.id) return true;
                        return false;
                    })) {
                        this.selectedRoles.push(role.id!);
                    }
                }
            });
        } else {
            groupData?.data?.items?.forEach((role: Role) => {
                if (Common.isNotNull(role) && Common.isNotNull(role.id)) {
                    if (this.selectedRoles.some((element: Guid) => {
                        if (element === role.id) return true;
                        return false;
                    })) {
                        this.selectedRoles.splice(this.selectedRoles.indexOf(role.id!), 1);
                    }
                }
            })
        }
    }

    async addGroupToRoles() {
        await this.context.role_Groups.addToRoles(this.groupId!, this.selectedRoles);
        this.slideService.popComponent(Role_GroupLinkComponent);
    }

    async addRoleToGroups() {
        await this.context.role_Groups.addToGroups(this.roleId!, this.selectedGroups);
        this.slideService.popComponent(Role_GroupLinkComponent);
    }

    async refreshRoles(): Promise<void> {
        if (this._rolesIsLoading) {
            this._rolesRefreshQueue++;
            return;
        }
        if (this.rolesDataGridComponent.instance !== undefined) {
            this.rolesDataGridComponent.instance.refresh();
        }
    }

    async refreshGroups(): Promise<void> {
        if (this._groupsIsLoading) {
            this._groupsRefreshQueue++;
            return;
        }
        if (this.groupsDataGridComponent.instance !== undefined) {
            this.groupsDataGridComponent.instance.refresh();
        }
    }

    protected async onRolesLoadingChanged(isLoading: boolean) {
        this._rolesIsLoading = isLoading;

        if (isLoading === false) {
            if (this._rolesRefreshQueue > 0) {
                this._rolesRefreshQueue = 0;
                await this.refreshRoles();
            }
        }
    }

    protected async onGroupsLoadingChanged(isLoading: boolean) {
        this._groupsIsLoading = isLoading;

        if (isLoading === false) {
            if (this._groupsRefreshQueue > 0) {
                this._groupsRefreshQueue = 0;
                await this.refreshGroups();
            }
        }
    }
}
