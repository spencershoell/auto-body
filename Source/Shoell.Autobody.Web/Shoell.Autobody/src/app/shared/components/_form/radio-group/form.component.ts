import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DxValidatorModule, DxRadioGroupModule } from 'devextreme-angular';

import { ValueChangedEvent } from 'devextreme/ui/radio_group';
import { EditorStyle, ValidationRule } from 'devextreme/common';

@Component({
    selector: 'form-radio-group',
    templateUrl: 'form.component.html',
    styleUrls: ['form.component.scss'],
    imports: [DxRadioGroupModule, DxValidatorModule]
})
export class FormRadioGroupComponent {
    @Input() isEditing = true;
    @Input() label = '';
    @Input() value?: boolean | null;
    @Input() items: any[] = [];
    @Input() displayExpr = '';
    @Input() valueExpr = '';

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
