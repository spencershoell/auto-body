import Guid from 'devextreme/core/guid';
import { DataFieldTypes, DataGridColumn } from 'src/app/classes';
import { BaseJoinModel, Group, IDataGrid, Role } from 'src/app/data/models';
import { Type } from 'class-transformer';

export class Role_Group extends BaseJoinModel {
    roleId?: Guid;
    groupId?: Guid;

    @Type(() => Role)
    role?: Role;
    @Type(() => Group)
    group?: Group;
}

export class Role_GroupODataStoreModel {
    public static getFieldTypes() {
        return {
            roleId: DataFieldTypes.Guid,
            groupId: DataFieldTypes.Guid,
        }
    }
}

export class Role_GroupDataGrid implements IDataGrid {
    roleId = new DataGridColumn({ dataField: "roleId", visible: false, showInColumnChooser: false });
    groupId = new DataGridColumn({ dataField: "groupId", visible: false, showInColumnChooser: false });

    buildColumns(columns: any): void {
        DataGridColumn.buildColumns(columns);
    }
}