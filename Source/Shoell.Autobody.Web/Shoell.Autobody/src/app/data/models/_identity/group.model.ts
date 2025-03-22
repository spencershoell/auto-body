import { BaseModelDataGrid, BaseModel, BaseModelODataStore, Role_Group, User_Group } from '..';
import { Type } from 'class-transformer';

export class Group extends BaseModel {
    @Type(() => Array<Role_Group>)
    roleGroups?: Role_Group[] = [];
    @Type(() => Array<User_Group>)
    userGroups?: User_Group[] = [];
}

export class GroupODataStoreModel extends BaseModelODataStore {
    public static override getFieldTypes() {
        let fieldTypes = super.getFieldTypes();
        return fieldTypes;
    }
}

export class GroupDataGrid extends BaseModelDataGrid {
    public override buildColumns(columns: any): void {
        return super.buildColumns(columns);
    }
}
