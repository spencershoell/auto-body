import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgIf } from '@angular/common';
import { DxButtonModule, DxNumberBoxModule, DxValidatorModule } from "devextreme-angular";
import { EditorStyle, ValidationRule } from 'devextreme/common';
import { ValueChangedEvent } from 'devextreme/ui/number_box';

@Component({
    selector: 'form-number-box',
    templateUrl: 'form.component.html',
    styleUrls: ['form.component.scss'],
    imports: [NgIf, DxButtonModule, DxNumberBoxModule, DxValidatorModule]
})
export class FormNumberBoxComponent {
    @Input() isEditing = true;

    @Input() text: number = 0;
    @Input() label: string = '';
    @Input() format: string = '';
    @Input() icon: string = '';
    @Input() stylingMode: EditorStyle = 'filled';

    @Input() validators: ValidationRule[] = [];
    @Input() validationGroup = '';

    @Input() value: number = 0;

    @Output() valueChange = new EventEmitter<number>();

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
