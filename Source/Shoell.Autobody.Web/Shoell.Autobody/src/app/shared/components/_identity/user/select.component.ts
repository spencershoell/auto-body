import { NgIf } from "@angular/common";
import { Component, ComponentRef } from "@angular/core";
import { DxButtonModule, DxDataGridModule, DxDropDownBoxModule, DxValidatorModule } from "devextreme-angular";
import DataSource from "devextreme/data/data_source";
import { BaseSelectComponent } from "src/app/shared/components";
import { takeUntil } from "rxjs";
import { AuthorizationService, DataEventService } from "src/app/shared/services";
import { SlidePanelService } from "src/app/layouts";
import { UserCreateComponent } from "./create.component";
import { EntityType } from "src/app/classes";
import { Context } from "src/app/data";
import { icon } from "@fortawesome/fontawesome-svg-core";
import { faPlus } from "@fortawesome/pro-solid-svg-icons";

@Component({
    selector: 'sh-user-select',
    templateUrl: './select.component.html',
    styleUrls: ['./select.component.scss'],
    standalone: true,
    imports: [NgIf, DxButtonModule, DxDataGridModule, DxDropDownBoxModule, DxValidatorModule]
})
export class UserSelectComponent extends BaseSelectComponent {
    override entityName = "User";
    override entityType = EntityType.User;

    fasPlus = icon(faPlus).html[0];

    constructor(
        protected dataEventService: DataEventService,
        override authorizationService: AuthorizationService,
        override slidePanelService: SlidePanelService,
        context: Context
    ) {
        super();
        this.dataSource = new DataSource({
            store: context.users,
            onLoadingChanged: this.onLoadingChanged.bind(this)
        });
    }

    override async ngOnInit(): Promise<void> {
        this.dataEventService.userChanged$
            .pipe(takeUntil(this._destroying$))
            .subscribe(async () => {
                await this.refresh();
            });

        await super.ngOnInit();
    }

    override create() {
        this.dropDownComponent.instance.close();
        this.slidePanelService.pushComponent({
            componentType: UserCreateComponent,
            setInputs: (componentRef: ComponentRef<UserCreateComponent>) => { },
            forceOverlay: true,
            title: `Create ${this.entityName}`
        });
    }

    protected override emitAdditionalChanges(selectedRowData: any): void { }

    override prepareFilters(filter: any[]): void { }
}
