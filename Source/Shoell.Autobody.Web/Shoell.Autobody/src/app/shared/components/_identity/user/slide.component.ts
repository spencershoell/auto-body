import { Component, SimpleChanges } from "@angular/core";
import { DxLoadIndicatorModule, DxTabPanelModule, DxToolbarModule } from "devextreme-angular";
import { User } from "src/app/data/models";
import { BaseSlideComponent } from "src/app/shared/components";
import { UserEditComponent } from "./edit.component";
import { SlidePanelService } from "src/app/layouts";
import { AuthorizationService, DataEventService } from "src/app/shared/services";
import { takeUntil } from "rxjs";
import { EntityType } from "src/app/classes";
import { NgIf } from "@angular/common";
import { User_GroupListComponent } from "../_join/user-group/list.component";
import ODataStore from "devextreme/data/odata/store";
import { Context } from "src/app/data";
import { LogListComponent } from "../../_system/logs/list.component";

@Component({
    selector: 'sh-user-slide',
    templateUrl: 'slide.component.html',
    styleUrls: ['slide.component.scss'],
    imports: [
        NgIf, DxLoadIndicatorModule, DxTabPanelModule, DxToolbarModule,
        UserEditComponent, User_GroupListComponent, LogListComponent
    ]
})
export class UserSlideComponent extends BaseSlideComponent<User> {
    override entityName: string = "User";
    override entityType: string = EntityType.User;

    override store: ODataStore;
    logStore: ODataStore;

    constructor(
        protected slidePanelService: SlidePanelService,
        protected dataEventService: DataEventService,
        override authorizationService: AuthorizationService,
        protected context: Context
    ) {
        super();
        this.store = context.users;
        this.logStore = context.users.getLogsStore(this.id);
    }

    public override async ngOnInit(): Promise<void> {
        await super.ngOnInit();
        this.logStore = this.context.users.getLogsStore(this.id);

        this.dataEventService.userChanged$
            .pipe(takeUntil(this._destroying$))
            .subscribe(async () => { await this.refresh(); });
    }

    override async ngOnChanges(changes: SimpleChanges): Promise<void> {
        await super.ngOnChanges(changes);
        if (changes['id']) {
            this.logStore = this.context.users.getLogsStore(this.id);
        }
    }

    onDeleted() {
        this.slidePanelService.popComponent(UserSlideComponent);
    }

    override buildSelect(): any[] {
        let select = super.buildSelect();
        select.push("name");
        return select;
    }

    override prepareFilters(filter: any[]) { }
}

