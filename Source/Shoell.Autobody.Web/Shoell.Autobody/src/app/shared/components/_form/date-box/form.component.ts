import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DxDateBoxModule, DxValidatorModule } from 'devextreme-angular';

import { ValueChangedEvent } from 'devextreme/ui/date_box';
import { EditorStyle, ValidationRule } from 'devextreme/common';

@Component({
    selector: 'form-date-box',
    templateUrl: 'form.component.html',
    styleUrls: ['form.component.scss'],
    imports: [DxDateBoxModule, DxValidatorModule]
})
export class FormDateBoxComponent {
    @Input() isEditing = true;
    @Input() label = '';
    @Input() value!: string | Date | number;
    @Input() validators: ValidationRule[] = [];
    @Input() validationGroup = '';
    @Input() stylingMode: EditorStyle = 'filled';

    @Output() valueChange: EventEmitter<string | Date | number> = new EventEmitter();

    get _stylingMode() {
        if (this.isEditing) {
            return 'outlined';
        }
        return this.stylingMode;
    }

    onValueChanged = (e: ValueChangedEvent) => {
        const { value } = e;

        this.value = value;
        this.valueChange.emit(this.value);
    };

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
