import { DataGridColumn } from "src/app/classes";

export interface IDataGrid {
    buildColumns(columns: DataGridColumn[]): void;
}