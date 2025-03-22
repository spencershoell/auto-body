import Guid from "devextreme/core/guid";
import { DataFieldTypes, DataGridColumn } from "src/app/classes";
import { IDataGrid, Role_Group, User_Role } from "..";
import { Type } from 'class-transformer';

export class Role {
    id?: Guid;
    rowId?: number;

    name?: string;
    operation?: string;
    target?: string;

    @Type(() => Array<Role_Group>)
    roleGroups?: Role_Group[] = [];
    @Type(() => Array<User_Role>)
    userRoles?: User_Role[] = [];
}

export class RoleODataStoreModel {
    public static getFieldTypes(): any {
        return {
            id: DataFieldTypes.Guid,
            rowId: DataFieldTypes.Int,
            name: DataFieldTypes.String,
            operation: DataFieldTypes.String,
            target: DataFieldTypes.String
        }
    }
}

export class RoleDataGrid implements IDataGrid {
    id = new DataGridColumn({ dataField: "id", visible: false, showInColumnChooser: false });
    rowId = new DataGridColumn({ dataField: "rowId", visible: false, showInColumnChooser: false });
    name = new DataGridColumn({ dataField: "name", sortIndex: 0, sortOrder: "asc" });
    operation = new DataGridColumn({ dataField: "operation" });
    target = new DataGridColumn({ dataField: "target" });

    buildColumns(columns: any): void {
        DataGridColumn.buildColumns(columns);
    }
}