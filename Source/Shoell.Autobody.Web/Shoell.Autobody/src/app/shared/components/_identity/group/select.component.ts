import { Component, ComponentRef } from "@angular/core";
import { DxButtonModule, DxDataGridModule, DxDropDownBoxModule, DxValidatorModule } from "devextreme-angular";
import DataSource from "devextreme/data/data_source";
import { BaseSelectComponent } from "src/app/shared/components";
import { takeUntil } from "rxjs";
import { AuthorizationService, DataEventService } from "src/app/shared/services";
import { SlidePanelService } from "src/app/layouts";
import { GroupCreateComponent } from "./create.component";
import { EntityType } from "src/app/classes";
import { Context } from "src/app/data";
import { icon } from "@fortawesome/fontawesome-svg-core";
import { faPlus } from "@fortawesome/pro-solid-svg-icons";
import { NgIf } from "@angular/common";

@Component({
    selector: 'sh-group-select',
    templateUrl: 'select.component.html',
    styleUrls: ['select.component.scss'],
    standalone: true,
    imports: [NgIf, DxButtonModule, DxDataGridModule, DxDropDownBoxModule, DxValidatorModule]
})
export class GroupSelectComponent extends BaseSelectComponent {
    override entityName = "Group";
    override entityType = EntityType.Group;

    fasPlus = icon(faPlus).html[0];

    constructor(
        protected dataEventService: DataEventService,
        override authorizationService: AuthorizationService,
        override slidePanelService: SlidePanelService,
        context: Context
    ) {
        super();
        this.dataSource = new DataSource({
            store: context.groups,
            onLoadingChanged: this.onLoadingChanged.bind(this)
        });
    }

    override async ngOnInit(): Promise<void> {
        this.dataEventService.groupChanged$
            .pipe(takeUntil(this._destroying$))
            .subscribe(async () => {
                await this.refresh();
            });

        await super.ngOnInit();
    }

    override create() {
        this.dropDownComponent.instance.close();
        this.slidePanelService.pushComponent({
            componentType: GroupCreateComponent,
            setInputs: (componentRef: ComponentRef<GroupCreateComponent>) => { },
            forceOverlay: true,
            title: `Create ${this.entityName}`
        });
    }

    override emitAdditionalChanges(selectedRowData: any): void { }

    override prepareFilters(filter: any[]): void { }
}