import { Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges, ViewChild } from "@angular/core";
import { DxDataGridComponent } from "devextreme-angular";
import DataSource from "devextreme/data/data_source";
import { ExportingEvent, RowClickEvent } from "devextreme/ui/data_grid";
import { exportDataGrid as exportDataGridToPdf } from 'devextreme/pdf_exporter';
import { exportDataGrid as exportDataGridToXLSX } from 'devextreme/excel_exporter';
import { Workbook } from 'exceljs';
import { saveAs } from 'file-saver-es';
import { jsPDF } from 'jspdf';
import { Subject } from "rxjs";
import { AuthorizationService } from "src/app/shared/services";
import { SlidePanelService } from "src/app/layouts";
import { IDataGrid } from "src/app/data";
import { DataGridColumn } from "src/app/classes";
import { SelectionChangedEvent } from "devextreme/ui/drop_down_button";
import { CoreComponent } from ".";

class Status {
    static All = 'All';
    static Active = 'Active';
    static Archived = 'Archived';
}

@Component({
    template: '<div></div>',
    standalone: false
})
export abstract class BaseListComponent<TDataGrid extends IDataGrid> extends CoreComponent implements OnChanges, OnDestroy, OnInit {
    protected _dataGrid!: TDataGrid;

    protected abstract slidePanelService: SlidePanelService;
    protected currentStatus = Status.Active;


    @ViewChild(DxDataGridComponent, { static: false }) dataGridComponent!: DxDataGridComponent;

    @Input() showTitle: boolean = true;
    @Input() title!: string;

    filterStatusList = [Status.All, Status.Active, Status.Archived];

    get dataGrid(): TDataGrid { return this._dataGrid; }



    constructor() {
        super();
        // Implementation
        this.ngOnInit = this.ngOnInit.bind(this);
        this.onExporting = this.onExporting.bind(this);
        this.onRowClick = this.onRowClick.bind(this);

        // Public
        this.create = this.create.bind(this);
        this.filterByStatus = this.filterByStatus.bind(this);
    }

    // Implementation
    async ngOnInit(): Promise<void> {
        if (this.title == undefined) {
            this.title = this.pluralizedEntityName;
        }
    }

    onExporting(e: ExportingEvent) {
        if (e.format === 'pdf') {
            const doc = new jsPDF();
            exportDataGridToPdf({
                jsPDFDocument: doc,
                component: e.component,
            }).then(() => {
                doc.save(`${this.pluralizedEntityName}.pdf`);
            });
        } else {
            const workbook = new Workbook();
            const worksheet = workbook.addWorksheet(this.pluralizedEntityName);

            exportDataGridToXLSX({
                component: e.component,
                worksheet,
                autoFilterEnabled: true,
            }).then(() => {
                workbook.xlsx.writeBuffer().then((buffer: any) => {
                    saveAs(new Blob([buffer], { type: 'application/octet-stream' }), `${this.pluralizedEntityName}.xlsx`);
                });
            });
            e.cancel = true;
        }
    }

    abstract onRowClick(e: RowClickEvent): void;

    // Public
    abstract create(): void;

    async filterByStatus(e: SelectionChangedEvent) {
        this.currentStatus = e.item;
        await this.refresh();
    }

    override async refresh(): Promise<void> {
        this._dataGrid.buildColumns(this.columns);

        await super.refresh();

        this.dataGridComponent.instance.refresh();
    }

    // Protected
    protected override buildFilter(): any[] | undefined {
        let filter = super.buildFilter() ?? [];

        if (this.currentStatus === Status.All) {
            if (filter.length > 0) {
                filter.push("and");
            }
            filter.push([["isArchived"], "or", ["isArchived", false]]);
        }

        if (this.currentStatus === Status.Active) {
            if (filter.length > 0) {
                filter.push("and");
            }
            filter.push(["isArchived", false]);
        }

        if (this.currentStatus == Status.Archived) {
            if (filter.length > 0) {
                filter.push("and");
            }
            filter.push(["isArchived"]);
        }

        if (filter?.length > 0) {
            return filter;
        }

        return undefined;
    }

    protected override buildSelect(): any[] {
        let select = super.buildSelect();
        select.push("id", "rowId");
        Object.entries(this._dataGrid).forEach(([key, value]) => {
            let column = value as DataGridColumn;
            if (column.visible || column.showInColumnChooser) {
                select.push(key);
            }
        });
        return select;
    }
}