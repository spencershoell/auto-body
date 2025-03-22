import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgIf } from '@angular/common';
import { DxButtonModule, DxDropDownBoxModule, DxValidatorModule } from "devextreme-angular";
import { EditorStyle, ValidationRule } from 'devextreme/common';
import { ValueChangedEvent } from 'devextreme/ui/drop_down_box';

@Component({
    selector: 'form-drop-down-box',
    templateUrl: 'form.component.html',
    styleUrls: ['form.component.scss'],
    imports: [NgIf, DxButtonModule, DxDropDownBoxModule, DxValidatorModule]
})
export class FormDropDownBoxComponent {
    @Input() isEditing = true;

    @Input() label: string = '';
    @Input() icon: string = '';
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
