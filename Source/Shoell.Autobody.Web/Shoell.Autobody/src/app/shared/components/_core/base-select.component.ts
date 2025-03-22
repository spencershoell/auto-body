import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges, ViewChild } from "@angular/core";
import { DxDataGridComponent, DxDropDownBoxComponent } from "devextreme-angular";
import { ValidationRule } from "devextreme/common";
import DataSource from "devextreme/data/data_source";
import { SelectionChangedEvent } from "devextreme/ui/data_grid";
import { Subject, takeUntil } from "rxjs";
import { AuthorizationService } from "src/app/shared/services";
import Guid from "devextreme/core/guid";
import { SlidePanelService } from "src/app/layouts";
import { Common } from "src/app/classes";

@Component({
    template: '<div></div>',
    standalone: false
})
export abstract class BaseSelectComponent implements OnChanges, OnDestroy, OnInit {
    private _value: any;

    protected _supressChangeNotification = false;
    protected _createInstanceId: Guid;

    protected readonly _destroying$ = new Subject<void>();

    // Block first change from triggering a reload, but reload directly after
    protected _isLoading: boolean = true;
    protected _refreshQueue: number = 1;

    protected abstract authorizationService: AuthorizationService;
    protected abstract slidePanelService: SlidePanelService;

    @ViewChild(DxDropDownBoxComponent, { static: false }) dropDownComponent!: DxDropDownBoxComponent;
    @ViewChild(DxDataGridComponent, { static: false }) dataGridComponent!: DxDataGridComponent;

    @Input() width: number = 600;
    @Input() set value(value: any) {
        this._value = value;
    }

    @Input() disabled: boolean = false;
    @Input() filter: any;
    @Input() icon: string = '';
    @Input() isEditing = true;
    @Input() label: string = '';
    @Input() validators: ValidationRule[] = [];
    @Input() validationGroup = '';

    @Output() onSelectionChanged = new EventEmitter<string>();
    @Output() valueChange = new EventEmitter<any>();

    abstract entityName: string;
    abstract entityType: string;

    dataSource!: DataSource;
    dropDownOptions = { width: 600 };

    get value(): any { return this._value; }

    get stylingMode() {
        if (this.isEditing) {
            return 'outlined';
        }
        return 'filled';
    }

    get canCreate(): boolean {
        return this.authorizationService.canCreate(this.entityType);
    }

    constructor() {
        this._createInstanceId = new Guid();

        // Implementation
        this.ngOnChanges = this.ngOnChanges.bind(this);
        this.ngOnDestroy = this.ngOnDestroy.bind(this);
        this.ngOnInit = this.ngOnInit.bind(this);

        // Event Handlers
        this.onLoadingChanged = this.onLoadingChanged.bind(this);
        this.onSelectionChangedHandler = this.onSelectionChangedHandler.bind(this);
        this.onValueChangedEvent = this.onValueChangedEvent.bind(this);

        // Public
        this.create = this.create.bind(this);
        this.refresh = this.refresh.bind(this);

        // Protected
        this.buildFilter = this.buildFilter.bind(this);
        this.buildSelect = this.buildSelect.bind(this);
        this.emitAdditionalChanges = this.emitAdditionalChanges.bind(this);
        this.prepareFilters = this.prepareFilters.bind(this);
    }

    // Implementation
    async ngOnChanges(changes: SimpleChanges): Promise<void> {
        if (Common.isNotNull(this.width)) {
            this.dropDownOptions.width = this.width;
        }

        if (changes['value'] && changes['value'].currentValue === changes['value'].previousValue) {
            this._supressChangeNotification = true;
        }

        await this.refresh();
    }

    ngOnDestroy(): void {
        this._destroying$.unsubscribe();
    }

    async ngOnInit(): Promise<void> {
        this.slidePanelService.componentRemoved$
            .pipe(takeUntil(this._destroying$))
            .subscribe(async (instanceId: Guid) => {
                if (instanceId.valueOf() == this._createInstanceId.valueOf()) {
                    this.dropDownComponent.instance.open();
                }
            });

        await this.refresh();
    }

    // Event Handlers
    async onLoadingChanged(isLoading: boolean) {
        this._isLoading = isLoading;

        if (isLoading === false) {
            if (this._refreshQueue > 0) {
                this._refreshQueue = 0;
                await this.refresh();
            }
        }
    }

    onSelectionChangedHandler(e: SelectionChangedEvent) {
        if (e.selectedRowKeys.length > 0) {
            this.valueChange.emit(e.selectedRowKeys[0]);
            this.onSelectionChanged.emit(e.selectedRowKeys[0]);

            this._supressChangeNotification = true;

            if (e.selectedRowsData.length > 0) {
                this.emitAdditionalChanges(e.selectedRowsData[0]);
            }

            this.dropDownComponent.instance.close();
        }
    }

    onValueChangedEvent(e: Event | any) {
        if (e.value == null && this.dataGridComponent != null) {
            this.dataGridComponent.instance.clearSelection();
            this.dataGridComponent.instance.clearFilter("search");
        }

        if (this._supressChangeNotification) {
            this._supressChangeNotification = false;
            return;
        }
        this.valueChange.emit(e.value);
        this.onSelectionChanged.emit(e.value);
    }

    // Public
    abstract create(): void;

    async refresh(): Promise<void> {
        if (this._isLoading) {
            this._refreshQueue++;
            return;
        }
        this.dataSource.filter(this.buildFilter());
        this.dataSource.select(this.buildSelect());
        if (this.dataGridComponent != null) {
            this.dataGridComponent.instance.refresh();
        }
    }

    // Protected
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
        let select = [];
        select.push("id", "name");
        return select;
    }

    protected abstract emitAdditionalChanges(selectedRowData: any): void;
    protected abstract prepareFilters(filter: any[]): void;
}