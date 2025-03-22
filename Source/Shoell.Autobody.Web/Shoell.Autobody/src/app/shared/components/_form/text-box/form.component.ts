import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgIf } from '@angular/common';
import { DxButtonModule, DxTextBoxModule, DxValidatorModule } from "devextreme-angular";
import { EditorStyle, ValidationRule } from 'devextreme/common';
import { ValueChangedEvent } from 'devextreme/ui/text_box';

@Component({
    selector: 'form-text-box',
    templateUrl: 'form.component.html',
    styleUrls: ['form.component.scss'],
    imports: [NgIf, DxButtonModule, DxTextBoxModule, DxValidatorModule]
})
export class FormTextBoxComponent {
    @Input() isEditing = true;

    @Input() text: string = '';
    @Input() label: string = '';
    @Input() mask: string = '';
    @Input() maskRules: any;
    @Input() icon: string = '';
    @Input() stylingMode: EditorStyle = 'filled';

    @Input() validators: ValidationRule[] = [];
    @Input() validationGroup = '';

    @Input() value: string = '';

    @Output() valueChange = new EventEmitter<string>();

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
