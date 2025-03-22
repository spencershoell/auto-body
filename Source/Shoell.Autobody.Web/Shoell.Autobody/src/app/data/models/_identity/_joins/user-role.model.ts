import Guid from "devextreme/core/guid";
import { Role, User } from "src/app/data/models";
import { DataFieldTypes, DataGridColumn } from "src/app/classes";
import { IDataGrid } from "src/app/data/models";
import { Type } from "class-transformer";

export class User_Role {
    userId?: Guid;
    roleId?: Guid;
    rowId?: number;

    @Type(() => User)
    user?: User;
    @Type(() => Role)
    role?: Role;
}

export class User_RoleODataModel {
    public static getFieldTypes() {
        return {
            userId: DataFieldTypes.Guid,
            roleId: DataFieldTypes.Guid,
        }
    }
}

export class User_RoleDataGrid implements IDataGrid {
    userId = new DataGridColumn({ dataField: "userId", visible: false, showInColumnChooser: false });
    roleId = new DataGridColumn({ dataField: "roleId", visible: false, showInColumnChooser: false });

    buildColumns(columns: any): void {
        DataGridColumn.buildColumns(columns);
    }
}