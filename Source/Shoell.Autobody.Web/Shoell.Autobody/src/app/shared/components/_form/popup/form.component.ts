import { Component, Input, ViewChild } from '@angular/core';
import { DxButtonModule, DxToolbarModule, DxPopupModule, DxValidationGroupModule, DxValidationGroupComponent } from 'devextreme-angular';
import { ScreenService } from '../../../services';
import { AsyncPipe } from '@angular/common';

@Component({
    selector: 'form-popup',
    templateUrl: 'form.component.html',
    styleUrls: ['form.component.scss'],
    imports: [AsyncPipe, DxButtonModule, DxToolbarModule, DxPopupModule, DxValidationGroupModule]
})

export class FormPopupComponent {
    @ViewChild('validationGroup', { static: true }) validationGroup!: DxValidationGroupComponent;

    @Input() titleText = '';

    popupVisible = false;

    constructor(protected screen: ScreenService) { }

    onCancelClick = () => {
        this.validationGroup.instance.reset();
        this.popupVisible = false;
    }

    onSaveClick = () => {
        if (!this.validationGroup.instance.validate().isValid) return;
        this.validationGroup.instance.reset();
        this.popupVisible = false;
    }
}