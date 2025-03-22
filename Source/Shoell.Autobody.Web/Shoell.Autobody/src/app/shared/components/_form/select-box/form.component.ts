import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgIf } from '@angular/common';
import { DxButtonModule, DxSelectBoxModule, DxValidatorModule } from "devextreme-angular";
import { EditorStyle, ValidationRule } from 'devextreme/common';
import { ValueChangedEvent } from 'devextreme/ui/select_box';

@Component({
    selector: 'form-select-box',
    templateUrl: 'form.component.html',
    styleUrls: ['form.component.scss'],
    imports: [NgIf, DxButtonModule, DxSelectBoxModule, DxValidatorModule]
})
export class FormSelectBoxComponent {
    @Input() isEditing = true;

    @Input() label: string = '';
    @Input() icon: string = '';
    @Input() displayExpr: ((item: any) => string) | string | undefined = 'name';
    @Input() valueExpr: ((item: any) => string | number | boolean) | string = 'id';
    @Input() searchExpr: Function | string | Array<Function | string> = '';
    @Input() searchEnabled: boolean = true;
    @Input() dataSource: any[] = [];
    @Input() stylingMode: EditorStyle = 'filled';

    @Input() validators: ValidationRule[] = [];
    @Input() validationGroup = '';

    @Input() value: any;

    @Output() valueChange = new EventEmitter<any>();

    valueChanged(e: ValueChangedEvent) {
        this.valueChange.emit(e.value);
    }

    get _stylingMode() {
        if (this.isEditing) {
            return 'outlined';
        }
        return this.stylingMode;
    }

    get elementClass(): string {
        let elementClass = 'form-editor';

        if (this.validators && this.validators.length > 0) {
            this.validators.every((validator: ValidationRule) => {
                if (validator.type === 'required') {
                    elementClass += ' required-validation';
                    return false;
                }
                return true;
            });
        }

        return elementClass;
    }
}
