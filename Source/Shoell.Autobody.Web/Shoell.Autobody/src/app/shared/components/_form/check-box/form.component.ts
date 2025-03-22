import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DxValidatorModule, DxCheckBoxModule } from 'devextreme-angular';

import { ValueChangedEvent } from 'devextreme/ui/check_box';
import { NgIf } from '@angular/common';
import { EditorStyle, ValidationRule } from 'devextreme/common';

@Component({
    selector: 'form-check-box',
    templateUrl: 'form.component.html',
    styleUrls: ['form.component.scss'],
    imports: [NgIf, DxCheckBoxModule, DxValidatorModule]
})
export class FormCheckBoxComponent {
    @Input() isEditing = true;
    @Input() label = '';
    @Input() value?: boolean | null;
    @Input() labelToRight: boolean = false;

    @Input() validators: ValidationRule[] = [];
    @Input() validationGroup = '';

    @Output() valueChange: EventEmitter<boolean | undefined | null> = new EventEmitter();

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
