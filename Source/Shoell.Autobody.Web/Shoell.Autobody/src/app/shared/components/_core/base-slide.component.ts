import { Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges, ViewChild } from "@angular/core";
import { DxTabPanelComponent } from "devextreme-angular";
import Guid from "devextreme/core/guid";
import ODataStore from "devextreme/data/odata/store";
import { Subject } from "rxjs";
import { Context } from "src/app/data";
import { AuthorizationService } from "src/app/shared/services";

@Component({
    template: '<div></div>',
    standalone: false
})
export abstract class BaseSlideComponent<TEntity> implements OnChanges, OnDestroy, OnInit {
    protected readonly _destroying$ = new Subject<void>();

    protected abstract authorizationService: AuthorizationService;

    @ViewChild(DxTabPanelComponent, { static: false }) tabPanelComponent!: DxTabPanelComponent;

    @Input() id!: Guid;

    model?: TEntity;
    abstract store: ODataStore;
    abstract entityName: string;
    abstract entityType: string;

    protected abstract context: Context;

    protected expand: string[] = [];
    protected filter: string[] = [];
    protected select: string[] = [];

    constructor() {
        // Implementation
        this.ngOnChanges = this.ngOnChanges.bind(this);
        this.ngOnDestroy = this.ngOnDestroy.bind(this);
        this.ngOnInit = this.ngOnInit.bind(this);

        // Public
        this.refresh = this.refresh.bind(this);

        // Protected
        this.buildSelect = this.buildSelect.bind(this);
        this.prepareFilters = this.prepareFilters.bind(this);
    }

    // Implementation
    async ngOnChanges(changes: SimpleChanges): Promise<void> {
        await this.refresh();
    }

    public ngOnDestroy(): void {
        this._destroying$.unsubscribe();
    }

    async ngOnInit(): Promise<void> {
        await this.refresh();
    }

    // Public
    async refresh(): Promise<void> {
        this.model = undefined;
        let response: any = await this.store
            .byKey(this.id, {
                expand: this.expand,
                select: this.buildSelect()
            });
        this.model = response;
    }

    // Protected
    protected buildSelect(): any[] {
        let select = [];
        select.push("id", "name");
        return select;
    }

    protected abstract prepareFilters(filter: any[]): void;
}