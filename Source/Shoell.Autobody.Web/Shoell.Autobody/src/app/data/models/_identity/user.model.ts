import Guid from "devextreme/core/guid";
import { DataFieldTypes, DataGridColumn } from "src/app/classes";
import { IDataGrid, Log, User_Group, User_Role } from "..";
import { Type } from "class-transformer";

export class User {
    id?: Guid;
    rowId?: number;
    name?: string;
    userName?: string;
    email?: string;
    phoneNumber?: string;
    isArchived?: boolean;

    externalId?: string;

    dateCreated?: Date;
    dateModified?: Date;

    createdById?: Guid;
    modifiedById?: Guid;

    @Type(() => Array<User>)
    createdBy?: User;
    @Type(() => Array<User>)
    modifiedBy?: User;

    @Type(() => Array<User_Group>)
    userGroups?: User_Group[] = [];
    @Type(() => Array<Log>)
    logs?: Log[] = [];
    @Type(() => Array<User_Role>)
    userRoles?: User_Role[] = [];
}

export class UserODataStoreModel {
    public static getFieldTypes(): any {
        return {
            id: DataFieldTypes.Guid,
            rowId: DataFieldTypes.Int,
            name: DataFieldTypes.String,
            userName: DataFieldTypes.String,
            phoneNumber: DataFieldTypes.String,
            externalId: DataFieldTypes.String,
            isArchived: DataFieldTypes.Boolean
        }
    }
}

export class UserDataGrid implements IDataGrid {
    id = new DataGridColumn({ dataField: "id", visible: false, showInColumnChooser: false });
    rowId = new DataGridColumn({ dataField: "rowId", visible: false, showInColumnChooser: false });
    name = new DataGridColumn({ dataField: "name" });
    userName = new DataGridColumn({ dataField: "userName" });
    email = new DataGridColumn({ dataField: "email" });
    phoneNumber = new DataGridColumn({ dataField: "phoneNumber" });
    isArchived = new DataGridColumn({ dataField: "isArchived" });
    externalId = new DataGridColumn({ dataField: "externalId" });
    dateCreated = new DataGridColumn({ dataField: "dateCreated" });
    dateModified = new DataGridColumn({ dataField: "dateModified" });

    buildColumns(columns: any): void {
        DataGridColumn.buildColumns(columns);
    }
}