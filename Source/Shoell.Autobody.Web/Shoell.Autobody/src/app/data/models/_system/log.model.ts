import Guid from "devextreme/core/guid";
import { User } from "..";
import { Type } from "class-transformer";
import { DataFieldTypes, DataGridColumn } from "src/app/classes";

export class Log {
    id?: Guid;
    rowId?: number;
    name?: string;
    type?: string;
    action?: string;
    description?: string;
    ipAddress?: string;
    userAgent?: string;
    dateCreated?: Date | string | number;

    entityId?: Guid;
    userId?: Guid;

    @Type(() => User)
    user?: User;
}

export class LogODataStoreModel {
    public static getFieldTypes() {
        return {
            id: DataFieldTypes.Guid,
            rowId: DataFieldTypes.Guid,
            name: DataFieldTypes.String,
            type: DataFieldTypes.String,
            action: DataFieldTypes.String,
            description: DataFieldTypes.String,
            ipAddress: DataFieldTypes.String,
            userAgent: DataFieldTypes.String,
            entityId: DataFieldTypes.Guid,
            userId: DataFieldTypes.Guid
        };
    }
}

export class LogDataGrid {
    id = new DataGridColumn({ dataField: "id", visible: false, showInColumnChooser: false });
    rowId = new DataGridColumn({ dataField: "rowId", visible: false, showInColumnChooser: false });
    name = new DataGridColumn({ dataField: "name" });
    type = new DataGridColumn({ dataField: "type" });
    action = new DataGridColumn({ dataField: "action" });
    description = new DataGridColumn({ dataField: "description" });
    ipAddress = new DataGridColumn({ dataField: "ipAddress" });
    userAgent = new DataGridColumn({ dataField: "userAgent", visible: false });
    dateCreated = new DataGridColumn({ dataField: "dateCreated", sortIndex: 0, sortOrder: "desc" });
    entityId = new DataGridColumn({ dataField: "entityId", visible: false, showInColumnChooser: false });
    userId = new DataGridColumn({ dataField: "userId", visible: true, showInColumnChooser: true });

    buildColumns(columns: any): void {
        DataGridColumn.buildColumns(columns);
    }
}