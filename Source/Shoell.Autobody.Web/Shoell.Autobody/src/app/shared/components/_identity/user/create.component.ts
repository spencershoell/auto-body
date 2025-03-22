import { NgIf } from "@angular/common";
import { Component } from "@angular/core";
import { DxButtonModule, DxFormModule, DxLoadIndicatorModule, DxScrollViewModule, DxToolbarModule, DxValidationSummaryModule } from "devextreme-angular";
import { SlidePanelService } from "src/app/layouts";
import { Context, User } from "src/app/data";
import { BaseCreateComponent, FormTextBoxComponent } from "src/app/shared/components";
import { EntityType } from "src/app/classes";
import { AuthorizationService } from "src/app/shared/services";
import ODataStore from "devextreme/data/odata/store";
import { icon } from "@fortawesome/fontawesome-svg-core";
import { faBan, faFloppyDisk } from "@fortawesome/pro-solid-svg-icons";
import { UserSelectComponent } from "./select.component";

@Component({
    selector: 'sh-user-create',
    templateUrl: 'create.component.html',
    styleUrls: ['create.component.scss'],
    imports: [
        NgIf, DxButtonModule, DxFormModule, DxLoadIndicatorModule, DxScrollViewModule, DxToolbarModule, DxValidationSummaryModule,
        FormTextBoxComponent, UserSelectComponent
    ]
})
export class UserCreateComponent extends BaseCreateComponent<User> {
    override entityName: string = "User";
    override entityType: string = EntityType.User;

    override store: ODataStore;
    override model!: User;

    fasFloppyDisk = icon(faFloppyDisk).html[0];
    fasBan = icon(faBan).html[0];

    constructor(
        protected slideService: SlidePanelService,
        override authorizationService: AuthorizationService,
        context: Context
    ) {
        super();
        this.validationGroup = `${EntityType.User}-CreateForm`;
        this.store = context.users;
    }

    override async reset(): Promise<void> {
        this.model = new User();
    }

    override async close(): Promise<void> {
        this.slideService.popComponent(UserCreateComponent);
    }
}
