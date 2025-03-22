import { NgIf } from "@angular/common";
import { Component } from "@angular/core";
import { DxButtonModule, DxFormModule, DxLoadIndicatorModule, DxScrollViewModule, DxToolbarModule, DxValidationSummaryModule } from "devextreme-angular";
import { SlidePanelService } from "src/app/layouts";
import { Context, Group } from "src/app/data";
import { BaseEditComponent, FormTextBoxComponent } from "src/app/shared/components";
import { AuthorizationService, DataEventService } from "src/app/shared/services";
import { takeUntil } from "rxjs";
import { EntityType } from "src/app/classes";
import Guid from "devextreme/core/guid";
import { GroupStore } from "src/app/data/stores";
import { icon } from "@fortawesome/fontawesome-svg-core";
import { faBoxArchive, faFloppyDisk, faPen, faTrash } from "@fortawesome/pro-solid-svg-icons";
import { faBoxArchive as farBoxArchive } from "@fortawesome/pro-regular-svg-icons";

@Component({
    selector: 'sh-group-edit',
    templateUrl: 'edit.component.html',
    styleUrls: ['edit.component.scss'],
    imports: [
        NgIf, DxButtonModule, DxFormModule, DxLoadIndicatorModule, DxScrollViewModule, DxToolbarModule, DxValidationSummaryModule,
        FormTextBoxComponent
    ]
})
export class GroupEditComponent extends BaseEditComponent<Group, Guid> {
    override entityName: string = "Group";
    override entityType: string = EntityType.Group;

    override store: GroupStore;
    override model!: Group;

    fasPen = icon(faPen).html[0];
    fasFloppyDisk = icon(faFloppyDisk).html[0];
    fasBoxArchive = icon(faBoxArchive).html[0];
    farBoxArchive = icon(farBoxArchive).html[0];
    fasTrash = icon(faTrash).html[0];

    constructor(
        protected dataEventService: DataEventService,
        protected slideService: SlidePanelService,
        override authorizationService: AuthorizationService,
        protected context: Context
    ) {
        super();
        this.validationGroup = `${EntityType.Group}-EditForm`;
        this.store = context.groups;
    }

    override async ngOnInit(): Promise<void> {
        await super.ngOnInit();

        this.dataEventService.groupChanged$
            .pipe(takeUntil(this._destroying$))
            .subscribe(async () => {
                if (!this.isEditing) {
                    await this.refresh();
                }
            });
    }

    override async close(): Promise<void> {
        this.slideService.popComponent(GroupEditComponent);
    }
}
