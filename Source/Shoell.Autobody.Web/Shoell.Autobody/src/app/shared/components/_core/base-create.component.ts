import { Component, OnChanges, OnInit, SimpleChanges, ViewChild } from "@angular/core";
import { DxFormComponent } from "devextreme-angular";
import validationEngine from "devextreme/ui/validation_engine";
import { AuthorizationService } from "src/app/shared/services";
import ODataStore from "devextreme/data/odata/store";
import Guid from "devextreme/core/guid";
import { Common } from "src/app/classes";

@Component({
    template: "<div></div>",
    standalone: false
})
export abstract class BaseCreateComponent<TEntity> implements OnChanges, OnInit {
    protected _createInstanceId: Guid;

    protected abstract authorizationService: AuthorizationService;

    @ViewChild(DxFormComponent, { static: false }) formComponent!: DxFormComponent;
    abstract entityName: string;
    abstract entityType: string;

    abstract store: ODataStore;
    abstract model: TEntity;

    validationGroup: string;

    isSaving: boolean = false;

    get canCreate(): boolean {
        return this.authorizationService.canCreate(this.entityType);
    }

    constructor() {
        this._createInstanceId = new Guid();
        this.validationGroup = `CreateForm`;

        // Implementation
        this.ngOnChanges = this.ngOnChanges.bind(this);
        this.ngOnInit = this.ngOnInit.bind(this);

        // Public
        this.close = this.close.bind(this);
        this.reset = this.reset.bind(this);
        this.save = this.save.bind(this);
    }

    // Implementation
    async ngOnChanges(changes: SimpleChanges): Promise<void> {
        await this.reset();
    }

    async ngOnInit(): Promise<void> {
        await this.reset();
    }

    // Public
    abstract close(): Promise<void>;

    abstract reset(): Promise<void>;

    async save(): Promise<void> {
        this.isSaving = true;
        if (validationEngine.validateGroup(this.validationGroup).isValid) {
            try {
                await this.store.insert(this.model);
                this.close();
                Common.toastSuccess( `${this.entityName} created successfully`);
            } catch (error) {
                Common.toastError(`${this.entityName} creation failed. '${error}'`);
            }
        } else {
            Common.toastError(`Please fix validation errors`);
        }
        this.isSaving = false;
    }
}