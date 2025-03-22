import { Component } from "@angular/core";
import { DxButtonModule, DxFormModule, DxLoadIndicatorModule, DxScrollViewModule, DxToolbarModule, DxValidationSummaryModule } from "devextreme-angular";
import { SlidePanelService } from "src/app/layouts";
import { Context, User } from "src/app/data";
import { BaseEditComponent, FormTextBoxComponent } from "src/app/shared/components";
import { AuthorizationService, DataEventService } from "src/app/shared/services";
import { takeUntil } from "rxjs";
import { EntityType } from "src/app/classes";
import { NgIf } from "@angular/common";
import Guid from "devextreme/core/guid";
import { UserStore } from "src/app/data/stores";
import { icon } from "@fortawesome/fontawesome-svg-core";
import { faBoxArchive, faFloppyDisk, faPen, faTrash } from "@fortawesome/pro-solid-svg-icons";
import { faBoxArchive as farBoxArchive } from "@fortawesome/pro-regular-svg-icons";
@Component({
    selector: 'sh-user-edit',
    templateUrl: 'edit.component.html',
    styleUrls: ['edit.component.scss'],
    imports: [
        NgIf, DxButtonModule, DxFormModule, DxLoadIndicatorModule, DxScrollViewModule, DxToolbarModule, DxValidationSummaryModule,
        FormTextBoxComponent
    ]
})
export class UserEditComponent extends BaseEditComponent<User, Guid> {
    override entityName: string = "User";
    override entityType: string = EntityType.User;

    override store: UserStore;
    override model!: User;

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
        this.validationGroup = `${EntityType.User}-EditForm`;
        this.store = context.users;
    }

    override async ngOnInit(): Promise<void> {
        await super.ngOnInit();

        this.dataEventService.userChanged$
            .pipe(takeUntil(this._destroying$))
            .subscribe(async () => {
                if (!this.isEditing) {
                    await this.refresh();
                }
            });
    }

    override async close(): Promise<void> {
        this.slideService.popComponent(UserEditComponent);
    }
}
