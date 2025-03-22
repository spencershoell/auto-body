import Guid from 'devextreme/core/guid';
import { DataFieldTypes, DataGridColumn } from 'src/app/classes';
import { BaseJoinModel, Group, IDataGrid, User } from 'src/app/data/models';
import { Type } from 'class-transformer';

export class User_Group extends BaseJoinModel {
    userId?: Guid;
    groupId?: Guid;

    @Type(() => User)
    user?: User;
    @Type(() => Group)
    group?: Group;
}

export class User_GroupODataStoreModel {
    public static getFieldTypes() {
        return {
            userId: DataFieldTypes.Guid,
            groupId: DataFieldTypes.Guid,
        }
    }
}

export class User_GroupDataGrid implements IDataGrid {
    userId = new DataGridColumn({ dataField: "userId", visible: false, showInColumnChooser: false });
    groupId = new DataGridColumn({ dataField: "groupId", visible: false, showInColumnChooser: false });

    buildColumns(columns: any): void {
        DataGridColumn.buildColumns(columns);
    }
}