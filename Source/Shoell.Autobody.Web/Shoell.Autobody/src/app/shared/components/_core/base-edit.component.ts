import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges, ViewChild } from "@angular/core";
import { DxFormComponent } from "devextreme-angular";
import Guid from "devextreme/core/guid";
import { Subject } from "rxjs";
import { Common } from "src/app/classes";
import { AuthorizationService } from "src/app/shared/services";
import validationEngine from "devextreme/ui/validation_engine";
import { CoreStore } from "src/app/data/stores";
import { DeepPartial } from "devextreme/core";

@Component({
    template: '<div></div>',
    standalone: false
})
export abstract class BaseEditComponent<TEntity, TKey> implements OnChanges, OnDestroy, OnInit {
    protected _editInstanceId: Guid;
    protected _isEditing: boolean = false;
    protected _isArchiving: boolean = false;
    protected _pristine: boolean = true;

    protected readonly _destroying$ = new Subject<void>();

    protected expand: string[] = [];
    protected select: string[] = [];

    protected abstract authorizationService: AuthorizationService;

    @ViewChild(DxFormComponent, { static: false }) formComponent!: DxFormComponent;

    @Input() id!: TKey;
    @Input() inSlide: boolean = false;

    @Output() onDeleted = new EventEmitter<any>();

    abstract entityName: string;
    abstract entityType: string;

    abstract store: CoreStore<TEntity, TKey>;
    abstract model: TEntity;

    validationGroup: string;

    isArchiving: boolean = false;
    isConfirmingDelete: boolean = false;
    isDeleting: boolean = false;
    isLoading: boolean = true;
    isSaving: boolean = false;

    get isEditing(): boolean {
        return this._isEditing && !this.isSaving && !this.isDeleting && !this.isArchiving;
    }

    get pristine(): boolean {
        return this._pristine;
    }

    get canUpdate(): boolean {
        return this.authorizationService.canUpdate(this.entityType);
    }

    get canDelete(): boolean {
        return this.authorizationService.canDelete(this.entityType);
    }

    get canArchive(): boolean {
        return this.authorizationService.canArchive(this.entityType);
    }

    get canRestore(): boolean {
        return this.authorizationService.canRestore(this.entityType);
    }

    constructor() {
        this._editInstanceId = new Guid();
        this.validationGroup = "EditForm";

        // Implementation
        this.ngOnChanges = this.ngOnChanges.bind(this);
        this.ngOnDestroy = this.ngOnDestroy.bind(this);
        this.ngOnInit = this.ngOnInit.bind(this);

        // Public
        this.archive = this.archive.bind(this);
        this.close = this.close.bind(this);
        this.delete = this.delete.bind(this);
        this.fieldDataChanged = this.fieldDataChanged.bind(this);
        this.refresh = this.refresh.bind(this);
        this.restore = this.restore.bind(this);
        this.save = this.save.bind(this);
        this.toggleConfirmDelete = this.toggleConfirmDelete.bind(this);
        this.toggleEdit = this.toggleEdit.bind(this);
    }

    // Implementation
    async ngOnChanges(changes: SimpleChanges): Promise<void> {
        if (Common.isNotNull(changes["id"]) && Common.isNotNull(changes["id"].currentValue)) {
            this.id = changes["id"].currentValue;
            await this.refresh();
        }
    }

    ngOnDestroy(): void {
        this._destroying$.unsubscribe();
    }

    async ngOnInit(): Promise<void> { }

    // Public
    async archive(): Promise<void> {
        this.isArchiving = true;
        try {
            await this.store.archive(this.id);
            Common.toastSuccess(`${this.entityName} archived successfully`);
        } catch (error) {
            Common.toastError(`${this.entityName} archive failed. '${error}'`);
        }

        this.isArchiving = false;
    }

    abstract close(): Promise<void>;

    async delete() {
        this.isConfirmingDelete = false;
        this.isDeleting = true;

        try {
            await this.store.remove(this.id);
            this.isDeleting = false;
            Common.toastSuccess(`${this.entityName} deleted successfully`);
            this.onDeleted.emit();
            this.close();
        } catch (error) {
            Common.toastError(`${this.entityName} delete failed. '${error}'`);
            this.isDeleting = false;
            this.isConfirmingDelete = true;
        }
    }

    fieldDataChanged() {
        this._pristine = false;
    }

    async refresh() {
        this._isEditing = false;
        this.isConfirmingDelete = false;
        this.isDeleting = false;
        this.isLoading = true;
        this.isSaving = false;
        this.model = {} as TEntity;
        try {
            this.model = await this.store.byKey(this.id, { expand: this.expand, select: this.select });
            this.isLoading = false;
            this._pristine = true;
        } catch {
            this.isLoading = false;
        }
    }

    async restore(): Promise<void> {
        this.isArchiving = true;
        try {
            await this.store.restore(this.id);
            Common.toastSuccess(`${this.entityName} restored successfully`);
        } catch (error) {
            Common.toastError(`${this.entityName} restore failed. '${error}'`);
        }

        this.isArchiving = false;
    }

    async save(): Promise<void> {
        this.isSaving = true;
        if (validationEngine.validateGroup(this.validationGroup).isValid) {
            try {
                await this.store.update(this.id, this.model as DeepPartial<TEntity>);
                Common.toastSuccess(`${this.entityName} edited successfully`);
                this.isSaving = false;
                await this.close();
            } catch (error) {
                Common.toastError(`${this.entityName} edit failed. '${error}'`);
            }
            this.isSaving = false;
        } else {
            Common.toastError(`Please fix validation errors`);
            this.isSaving = false;
        }
    }

    toggleConfirmDelete() {
        this._isEditing = false;
        this.isConfirmingDelete = !this.isConfirmingDelete;
    }

    async toggleEdit(forceEdit = false) {
        this.isConfirmingDelete = false;
        if (this._isEditing && !this.pristine) {
            await this.refresh();
            return;
        }
        this._isEditing = !this._isEditing;
        if (forceEdit) {
            this._isEditing = true;
        }
    }
}