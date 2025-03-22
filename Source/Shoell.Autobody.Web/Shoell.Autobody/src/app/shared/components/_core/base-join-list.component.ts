import { Component, Input, OnChanges, SimpleChanges, ViewChild } from "@angular/core";
import { DxDataGridComponent } from "devextreme-angular";
import DataSource from "devextreme/data/data_source";
import { DataGridColumn } from "src/app/classes";
import { IDataGrid } from "src/app/data";
import { AuthorizationService } from "src/app/shared/services";

@Component({
    template: '<div></div>',
    standalone: false
})
export abstract class BaseJoinListComponent<TDataGrid extends IDataGrid> implements OnChanges {
    protected _dataGrid!: TDataGrid;

    // Block first change from triggering a reload, but reload directly after
    protected _isLoading: boolean = true;
    protected _refreshQueue: number = 1;

    protected abstract authorizationService: AuthorizationService;

    @ViewChild(DxDataGridComponent, { static: false }) dataGridComponent!: DxDataGridComponent;

    @Input() filter: any;
    @Input() columns: any = [];
    @Input() disabled: boolean = false;

    abstract entityType1: string;
    abstract entityType2: string;

    dataSource!: DataSource;

    isEditing: boolean = false;

    get dataGrid(): TDataGrid { return this._dataGrid; }

    get canModify(): boolean {
        return this.authorizationService.canModify(this.entityType1)
            || this.authorizationService.canModify(this.entityType2);
    }

    constructor() {
        // Event Handlers
        this.ngOnChanges = this.ngOnChanges.bind(this);
        this.onLoadingChanged = this.onLoadingChanged.bind(this);

        // Public Methods
        this.link = this.link.bind(this);
        this.refresh = this.refresh.bind(this);

        // Protected Methods
        this.buildFilter = this.buildFilter.bind(this);
        this.buildSelect = this.buildSelect.bind(this);
        this.prepareFilters = this.prepareFilters.bind(this);
    }

    // Event Handlers
    async ngOnChanges(changes: SimpleChanges): Promise<void> {
        await this.refresh();
    }

    protected onLoadingChanged(isLoading: boolean) {
        this._isLoading = isLoading;

        if (isLoading === false) {
            if (this._refreshQueue > 0) {
                this._refreshQueue = 0;
                this.refresh();
            }
        }
    }

    // Public Methods
    abstract link(): void;

    async refresh(): Promise<void> {
        this.dataSource.filter(this.buildFilter());
        this._dataGrid.buildColumns(this.columns);
        this.dataSource.select(this.buildSelect());
        this.dataGridComponent?.instance.refresh();
    }

    async saveEditData(): Promise<void> {
        if (this.dataGridComponent && this.dataGridComponent.instance) {
            await this.dataGridComponent.instance.saveEditData();
            this.isEditing = false;
        }
    }

    // Protected Methods
    protected buildFilter(): any[] | undefined {
        let filter: any[] = [];

        this.prepareFilters(filter);

        if (this.filter?.length > 0) {
            if (filter.length > 0) {
                filter.push("and");
            }
            filter.push(this.filter);
        }

        if (filter?.length > 0) {
            return filter;
        }

        return undefined;
    }

    protected buildSelect(): any[] {
        let select: any[] = [];
        Object.entries(this._dataGrid).forEach(([key, value]) => {
            let column = value as DataGridColumn;
            if (column.visible || column.showInColumnChooser) {
                select.push(key);
            }
        });
        return select;
    }

    protected abstract prepareFilters(filter: any[]): void;
}