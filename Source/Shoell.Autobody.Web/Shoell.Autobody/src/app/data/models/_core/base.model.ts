import { Type } from "class-transformer";
import Guid from "devextreme/core/guid";
import { DataFieldTypes, DataGridColumn } from "src/app/classes";
import { IDataGrid, User } from "..";

export class BaseModel {
    id?: Guid;
    rowId?: number;

    name?: string;
    description?: string;
    isArchived?: boolean;

    dateCreated?: Date | string | number;
    dateModified?: Date | string | number;

    createdById?: Guid;
    modifiedById?: Guid;

    @Type(() => Array<User>)
    createdBy?: User;
    @Type(() => Array<User>)
    modifiedBy?: User;

    constructor() {
        this.id = new Guid();
    }
}

export class BaseModelODataStore {
    public static getFieldTypes(): any {
        return {
            id: DataFieldTypes.Guid,
            rowId: DataFieldTypes.Int,
            name: DataFieldTypes.String,
            isArchived: DataFieldTypes.Boolean,
            description: DataFieldTypes.String,
            createdById: DataFieldTypes.Guid,
            modifiedById: DataFieldTypes.Guid,
        }
    }
}

export class BaseModelDataGrid implements IDataGrid {
    id = new DataGridColumn({ dataField: "id", visible: false, showInColumnChooser: false });
    rowId = new DataGridColumn({ dataField: "rowId", visible: false, showInColumnChooser: false });
    name = new DataGridColumn({ dataField: "name", sortIndex: 0, sortOrder: "asc" });
    description = new DataGridColumn({ dataField: "description", visible: false, showInColumnChooser: true });
    isArchived = new DataGridColumn({ dataField: "isArchived" });
    dateCreated = new DataGridColumn({ dataField: "dateCreated", visible: false, showInColumnChooser: true });
    dateModified = new DataGridColumn({ dataField: "dateModified", visible: false, showInColumnChooser: true });
    createdById = new DataGridColumn({ dataField: "createdById", visible: false, showInColumnChooser: true });
    modifiedById = new DataGridColumn({ dataField: "modifiedById", visible: false, showInColumnChooser: true });

    buildColumns(columns: any): void {
        DataGridColumn.buildColumns(columns);
    }
}