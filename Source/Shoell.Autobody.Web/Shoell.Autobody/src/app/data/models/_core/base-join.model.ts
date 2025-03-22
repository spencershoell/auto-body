import { DataFieldTypes, DataGridColumn } from "src/app/classes";
import { IDataGrid } from "..";

export abstract class BaseJoinModel {
    rowId?: number;
}

export class BaseJoinODataModel {
    public static getFieldTypes(): any {
        return {
            rowId: DataFieldTypes.Int
        };
    }
}

export class BaseJoinDataGrid implements IDataGrid {
    rowId = new DataGridColumn({ dataField: "rowId", visible: false, showInColumnChooser: false });

    buildColumns(columns: any): void {
        DataGridColumn.buildColumns(columns);
    }
}