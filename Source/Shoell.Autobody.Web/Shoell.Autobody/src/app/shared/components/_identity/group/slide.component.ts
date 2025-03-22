import { Component, SimpleChanges } from "@angular/core";
import { DxLoadIndicatorModule, DxTabPanelModule, DxToolbarModule } from "devextreme-angular";
import { Group } from "src/app/data/models";
import { BaseSlideComponent } from "src/app/shared/components";
import { GroupEditComponent } from "./edit.component";
import { SlidePanelService } from "src/app/layouts";
import { AuthorizationService, DataEventService } from "src/app/shared/services";
import { takeUntil } from "rxjs";
import { Role_GroupListComponent } from "../_join/role-group/list.component";
import { User_GroupListComponent } from "../_join/user-group/list.component";
import { EntityType } from "src/app/classes";
import { NgIf } from "@angular/common";
import ODataStore from "devextreme/data/odata/store";
import { Context } from "src/app/data";
import { LogListComponent } from "../../_system/logs/list.component";

@Component({
    selector: 'sh-group-slide',
    templateUrl: 'slide.component.html',
    styleUrls: ['slide.component.scss'],
    imports: [
        NgIf, DxLoadIndicatorModule, DxTabPanelModule, DxToolbarModule,
        GroupEditComponent, Role_GroupListComponent, User_GroupListComponent, LogListComponent
    ]
})
export class GroupSlideComponent extends BaseSlideComponent<Group> {
    override entityName: string = "Group";
    override entityType: string = EntityType.Group;

    override store: ODataStore;
    logStore: ODataStore;

    constructor(
        protected slidePanelService: SlidePanelService,
        protected dataEventService: DataEventService,
        override authorizationService: AuthorizationService,
        protected context: Context
    ) {
        super();
        this.store = context.groups;
        this.logStore = context.groups.getLogsStore(this.id);
    }

    override async ngOnInit(): Promise<void> {
        await super.ngOnInit();
        this.logStore = this.context.groups.getLogsStore(this.id);

        this.dataEventService.groupChanged$
            .pipe(takeUntil(this._destroying$))
            .subscribe(async () => { await this.refresh(); });
    }

    override async ngOnChanges(changes: SimpleChanges): Promise<void> {
        await super.ngOnChanges(changes);
        if (changes['id']) {
            this.logStore = this.context.groups.getLogsStore(this.id);
        }
    }

    onDeleted() {
        this.slidePanelService.popComponent(GroupSlideComponent);
    }

    override buildSelect(): any[] {
        let select = super.buildSelect();
        select.push("name");
        return select;
    }

    override prepareFilters(filter: any[]) { }
}
