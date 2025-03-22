import { NgIf } from "@angular/common";
import { Component, ComponentRef, Input, OnInit, ViewChild } from "@angular/core";
import { DxButtonModule, DxDataGridComponent, DxDataGridModule } from 'devextreme-angular';
import Guid from 'devextreme/core/guid';
import DataSource from "devextreme/data/data_source";
import { Common } from 'src/app/classes';
import { SlidePanelService } from "src/app/layouts/slide-panel/slide-panel.services";
import { Context } from "src/app/data";
import { faLink, faPlus } from "@fortawesome/pro-solid-svg-icons";
import { icon } from "@fortawesome/fontawesome-svg-core";
import { GroupCreateComponent } from "../../group/create.component";
import { DataEventService } from "src/app/shared/services";
import { Subject, takeUntil } from "rxjs";
import { UserCreateComponent } from "../../user/create.component";

@Component({
    selector: 'sh-user-group-link',
    templateUrl: 'link.component.html',
    styleUrls: ['link.component.scss'],
    imports: [NgIf, DxButtonModule, DxDataGridModule]
})
export class User_GroupLinkComponent implements OnInit {
    protected readonly _destroying$ = new Subject<void>();

    @ViewChild('usersDataGrid', { static: false }) usersDataGridComponent!: DxDataGridComponent;
    @ViewChild('groupsDataGrid', { static: false }) groupsDataGridComponent!: DxDataGridComponent;

    // Block first change from triggering a reload, but reload directly after
    protected _usersIsLoading: boolean = true;
    protected _usersRefreshQueue: number = 1;
    protected _groupsIsLoading: boolean = true;
    protected _groupsRefreshQueue: number = 1;

    @Input() userId?: Guid;
    @Input() groupId?: Guid;

    get showUsers(): boolean { return Common.isNull(this.userId); }
    get showGroups(): boolean { return Common.isNull(this.groupId); }

    usersDataSource: DataSource;
    groupsDataSource: DataSource;

    selectedUsers: Guid[] = [];
    selectedGroups: Guid[] = [];

    fasLink = icon(faLink).html[0];
    fasPlus = icon(faPlus).html[0];

    constructor(
        protected context: Context,
        protected dataEventService: DataEventService,
        protected slideService: SlidePanelService
    ) {
        this.usersDataSource = new DataSource({
            store: context.users,
            select: ["id", "name", "externalId"],
            onLoadingChanged: this.onUserLoadingChanged.bind(this)
        });

        this.groupsDataSource = new DataSource({
            store: context.groups,
            select: ["id", "name"],
            onLoadingChanged: this.onGroupsLoadingChanged.bind(this)
        });

        this.ngOnInit = this.ngOnInit.bind(this);

        this.createGroup = this.createGroup.bind(this);
        this.createUser = this.createUser.bind(this);
        this.addGroupToUsers = this.addGroupToUsers.bind(this);
        this.addUserToGroups = this.addUserToGroups.bind(this);
        this.refreshUsers = this.refreshUsers.bind(this);
        this.refreshGroups = this.refreshGroups.bind(this);
        this.onUserLoadingChanged = this.onUserLoadingChanged.bind(this);
        this.onGroupsLoadingChanged = this.onGroupsLoadingChanged.bind(this);
    }

    public async ngOnInit(): Promise<void> {
        this.dataEventService.groupChanged$
            .pipe(takeUntil(this._destroying$))
            .subscribe(async () => { await this.refreshGroups(); });

        this.dataEventService.userChanged$
            .pipe(takeUntil(this._destroying$))
            .subscribe(async () => { await this.refreshUsers(); });
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

    createUser(): void {
        this.slideService.pushComponent({
            componentType: UserCreateComponent,
            setInputs: (componentRef: ComponentRef<UserCreateComponent>) => {
            },
            title: `Create User`,
            forceOverlay: true
        });
    }

    async addGroupToUsers() {
        await this.context.user_Groups.addToUsers(this.groupId!, this.selectedUsers);
        this.slideService.popComponent(User_GroupLinkComponent);
    }

    async addUserToGroups() {
        await this.context.user_Groups.addToGroups(this.userId!, this.selectedGroups);
        this.slideService.popComponent(User_GroupLinkComponent);
    }

    async refreshUsers(): Promise<void> {
        if (this._usersIsLoading) {
            this._usersRefreshQueue++;
            return;
        }

        await this.usersDataSource.reload();
        if (this.usersDataGridComponent.instance !== undefined) {
            this.usersDataGridComponent.instance.refresh();
        }
    }

    async refreshGroups(): Promise<void> {
        if (this._groupsIsLoading) {
            this._groupsRefreshQueue++;
            return;
        }

        await this.groupsDataSource.reload();
        if (this.groupsDataGridComponent.instance !== undefined) {
            this.groupsDataGridComponent.instance.refresh();
        }
    }

    protected async onUserLoadingChanged(isLoading: boolean) {
        this._usersIsLoading = isLoading;

        if (isLoading === false) {
            if (this._usersRefreshQueue > 0) {
                this._usersRefreshQueue = 0;
                await this.refreshUsers();
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