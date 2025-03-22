import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from "@angular/core";
import { NgClass, NgIf } from "@angular/common";
import { Router } from "@angular/router";
import Guid from "devextreme/core/guid";
import { DxButtonModule, DxLoadIndicatorModule, DxResponsiveBoxModule, DxTabPanelComponent, DxTabPanelModule, DxToolbarModule } from "devextreme-angular";
import { GroupEditComponent } from "./edit.component";
import { Group } from "src/app/data/models";
import { User_GroupListComponent } from "../_join/user-group/list.component";
import { Role_GroupListComponent } from "../_join/role-group/list.component";
import { Context } from "src/app/data";
import { LogListComponent } from "../../_system/logs/list.component";
import ODataStore from "devextreme/data/odata/store";
import { icon } from "@fortawesome/fontawesome-svg-core";
import { faRefresh, faArrowLeft } from "@fortawesome/pro-light-svg-icons";
import { ScreenService } from "src/app/shared/services";

@Component({
    selector: 'sh-group-detail',
    templateUrl: 'detail.component.html',
    styleUrls: ['detail.component.scss'],
    imports: [
        NgClass, NgIf, DxButtonModule, DxLoadIndicatorModule, DxResponsiveBoxModule, DxTabPanelModule, DxToolbarModule,
        GroupEditComponent, User_GroupListComponent, Role_GroupListComponent, LogListComponent
    ]
})
export class GroupDetailComponent implements OnChanges, OnInit {
    @Input() id!: Guid;

    @ViewChild(DxTabPanelComponent, { static: false }) tabPanel!: DxTabPanelComponent;


    model?: Group;

    logStore!: ODataStore;

    falRefresh = icon(faRefresh).html[0];
    falArrowLeft = icon(faArrowLeft).html[0];

    isSmallScreen = false;

    constructor(
        protected context: Context,
        protected router: Router,
        protected screenService: ScreenService
    ) {
        this.logStore = context.groups.getLogsStore(this.id);

        // Implementation
        this.ngOnChanges = this.ngOnChanges.bind(this);

        // Public
        this.goBack = this.goBack.bind(this);
        this.refresh = this.refresh.bind(this);
        this.updateScreenSize = this.updateScreenSize.bind(this);
    }

    // Implementation
    async ngOnChanges(changes: SimpleChanges): Promise<void> {
        if (changes["id"] != null && changes["id"].currentValue != null) {
            await this.refresh();
            this.logStore = this.context.groups.getLogsStore(this.id);
        }
    }

    async ngOnInit(): Promise<void> {
        this.updateScreenSize();
        await this.refresh();

        this.logStore = this.context.groups.getLogsStore(this.id);

        this.screenService.changed.subscribe(() => this.updateScreenSize());
    }

    // Public
    goBack(): void {
        this.router.navigate(['/group']);
    }

    async refresh(): Promise<void> {
        this.model = undefined;
        this.model = (await this.context.groups.load({ filter: ['id', this.id], select: ['id', 'name'] }))[0];
    }

    protected updateScreenSize() {
        this.isSmallScreen = this.screenService.sizes['screen-small'] || this.screenService.sizes['screen-x-small'];

        if (this.tabPanel != null) {
            if (!this.isSmallScreen && this.tabPanel.instance.option("selectedIndex") == 0) {
                this.tabPanel.instance.option("selectedIndex", 1);
            }
        }
    }
}