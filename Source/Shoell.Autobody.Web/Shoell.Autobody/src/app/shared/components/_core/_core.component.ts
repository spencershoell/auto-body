import { Component, Input, OnChanges, OnDestroy, SimpleChanges } from "@angular/core";
import { Subject } from "rxjs";
import { AuthorizationService } from "../../services";
import DataSource from "devextreme/data/data_source";
import { SelectDescriptor } from "devextreme/data";

@Component({
    template: '<div></div>',
    standalone: false
})
export abstract class CoreComponent implements OnChanges, OnDestroy {
    protected readonly _destroying$ = new Subject<void>();

    // Block first change from triggering a reload, but reload directly after
    protected _isLoading: boolean = true;
    protected _refreshQueue: number = 0;

    protected abstract authorizationService: AuthorizationService;

    @Input() filter: any[] = [];
    @Input() select: any[] = [];
    @Input() columns: any[] = [];
    @Input() disabled: boolean = false;

    dataSource!: DataSource;

    abstract entityName: string;
    abstract entityType: string;
    abstract pluralizedEntityName: string;

    get canCreate(): boolean {
        return this.authorizationService.canCreate(this.entityType);
    }

    get canRead(): boolean {
        return this.authorizationService.canRead(this.entityType);
    }

    get canUpdate(): boolean {
        return this.authorizationService.canUpdate(this.entityType);
    }

    get canDelete(): boolean {
        return this.authorizationService.canDelete(this.entityType);
    }

    get canModify(): boolean {
        return this.authorizationService.canModify(this.entityType);
    }

    get canRecycle(): boolean {
        return this.authorizationService.canRecycle(this.entityType);
    }

    get canRecover(): boolean {
        return this.authorizationService.canRecover(this.entityType);
    }

    get canPurge(): boolean {
        return this.authorizationService.canPurge(this.entityType);
    }

    get canArchive(): boolean {
        return this.authorizationService.canArchive(this.entityType);
    }

    get canRestore(): boolean {
        return this.authorizationService.canRestore(this.entityType);
    }

    constructor() {
        // Implementation
        this.ngOnChanges = this.ngOnChanges.bind(this);
        this.ngOnDestroy = this.ngOnDestroy.bind(this);

        // Public
        this.refresh = this.refresh.bind(this);

        // Protected
        this.buildFilter = this.buildFilter.bind(this);
        this.buildSelect = this.buildSelect.bind(this);
        this.onLoadingChanged = this.onLoadingChanged.bind(this);
        this.prepareFilter = this.prepareFilter.bind(this);
        this.prepareSelect = this.prepareSelect.bind(this);
    }

    // Implementation
    async ngOnChanges(changes: SimpleChanges): Promise<void> {
        await this.refresh();
    }

    ngOnDestroy(): void {
        this._destroying$.unsubscribe();
    }

    // Public
    async refresh(): Promise<void> {
        if (this._isLoading) {
            this._refreshQueue++;
            return;
        }

        this.dataSource.filter(this.buildFilter());
        this.dataSource.select(this.buildSelect());
    }

    // Protected
    protected buildFilter(): any[] | undefined {
        let filter: any[] = [];

        this.prepareFilter(filter);

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

        this.prepareSelect(select);

        if (this.select?.length > 0) {
            if (select.length > 0) {
                select.push("and");
            }
            select.push(this.select);
        }

        return select;
    }

    protected async onLoadingChanged(isLoading: boolean) {
        this._isLoading = isLoading;
        if (isLoading === false) {
            if (this._refreshQueue > 0) {
                this._refreshQueue = 0;
                await this.refresh();
            }
        }
    }

    protected abstract prepareFilter(filter: any[]): void;
    protected abstract prepareSelect(select: any[]): void;
}