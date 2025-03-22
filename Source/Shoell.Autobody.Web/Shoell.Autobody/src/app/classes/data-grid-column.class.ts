import { Common } from "./common.class";

export class DataGridColumn {
    dataField!: string;
    hidingPriority?: number;
    showInColumnChooser?: boolean;
    sortIndex?: number;
    sortOrder?: string;
    visible?: boolean;
    visibleIndex?: number;
    groupIndex?: number;
    excludeFromSelect?: boolean;

    constructor(defaultValues?: DataGridColumn) {
        this.hidingPriority = undefined;
        this.showInColumnChooser = true;
        this.sortIndex = undefined;
        this.sortOrder = undefined;
        this.visible = true;
        this.visibleIndex = undefined;
        this.groupIndex = undefined;

        if (Common.isNotNull(defaultValues)) {
            if (Common.isNotNull(defaultValues?.hidingPriority)) {
                this.hidingPriority = defaultValues?.hidingPriority;
            }
            if (Common.isNotNull(defaultValues?.showInColumnChooser)) {
                this.showInColumnChooser = defaultValues?.showInColumnChooser;
            }
            if (Common.isNotNull(defaultValues?.sortIndex)) {
                this.sortIndex = defaultValues?.sortIndex;
            }
            if (Common.isNotNull(defaultValues?.sortOrder)) {
                this.sortOrder = defaultValues?.sortOrder;
            }
            if (Common.isNotNull(defaultValues?.visible)) {
                this.visible = defaultValues?.visible!;
            }
            if (Common.isNotNull(defaultValues?.visibleIndex)) {
                this.visibleIndex = defaultValues?.visibleIndex;
            }
            if (Common.isNotNull(defaultValues?.groupIndex)) {
                this.groupIndex = defaultValues?.groupIndex;
            }
        }
    }

    public static buildColumns(columns: DataGridColumn[]) {
        let visibleIndex = 50;
        columns.forEach(column => {
            if (Common.isNotNull(column.dataField)) {
                if (Common.isNotNull(column.hidingPriority)) {
                    (this as any)[column.dataField].hidingPriority = column.hidingPriority;
                } else {
                    (this as any)[column.dataField].hidingPriority = 10;
                }
                if (Common.isNotNull(column.showInColumnChooser)) {
                    (this as any)[column.dataField].showInColumnChooser = column.showInColumnChooser;
                } else {
                    (this as any)[column.dataField].showInColumnChooser = true;
                }
                if (Common.isNotNull(column.sortIndex)) {
                    (this as any)[column.dataField].sortIndex = column.sortIndex;
                }
                if (Common.isNotNull(column.sortOrder)) {
                    (this as any)[column.dataField].sortOrder = column.sortOrder;
                }
                if (Common.isNotNull(column.visible)) {
                    (this as any)[column.dataField].visible = column.visible;
                } else {
                    (this as any)[column.dataField].visible = true;
                }
                if (Common.isNotNull(column.visibleIndex)) {
                    (this as any)[column.dataField].visible = column.visibleIndex;
                } else {
                    (this as any)[column.dataField].visibleIndex = visibleIndex;
                    visibleIndex++;
                }
            }
        });
    }
}
