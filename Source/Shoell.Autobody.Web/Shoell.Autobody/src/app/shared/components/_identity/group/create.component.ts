import { NgIf } from "@angular/common";
import { Component } from "@angular/core";
import { DxButtonModule, DxFormModule, DxLoadIndicatorModule, DxScrollViewModule, DxToolbarModule, DxValidationSummaryModule } from "devextreme-angular";
import { SlidePanelService } from "src/app/layouts";
import { Group } from "src/app/data/models";
import { BaseCreateComponent, FormTextBoxComponent } from "src/app/shared/components";
import { AuthorizationService } from "src/app/shared/services";
import { EntityType } from "src/app/classes";
import ODataStore from "devextreme/data/odata/store";
import { Context } from "src/app/data";
import { faBan, faFloppyDisk } from "@fortawesome/pro-solid-svg-icons";
import { icon } from "@fortawesome/fontawesome-svg-core";

@Component({
    selector: 'sh-group-create',
    templateUrl: 'create.component.html',
    styleUrls: ['create.component.scss'],
    imports: [
        NgIf, DxButtonModule, DxFormModule, DxLoadIndicatorModule, DxScrollViewModule, DxToolbarModule, DxValidationSummaryModule,
        FormTextBoxComponent
    ]
})
export class GroupCreateComponent extends BaseCreateComponent<Group> {
    override entityName: string = "Group";
    override entityType: string = EntityType.Group;

    override store: ODataStore;
    override model!: Group;

    fasFloppyDisk = icon(faFloppyDisk).html[0];
    fasBan = icon(faBan).html[0];

    constructor(
        protected slideService: SlidePanelService,
        override authorizationService: AuthorizationService,
        context: Context
    ) {
        super();
        this.validationGroup = `${EntityType.Group}-CreateForm`;
        this.store = context.groups;
    }

    override async reset(): Promise<void> {
        this.model = new Group();
    }

    override async close(): Promise<void> {
        this.slideService.popComponent(GroupCreateComponent);
    }
}