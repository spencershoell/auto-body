import { Component, Input } from '@angular/core';
import { faCircleUser } from '@fortawesome/pro-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { DxDropDownButtonModule, DxListModule } from 'devextreme-angular';
import { icon } from '@fortawesome/fontawesome-svg-core';
import { NgIf } from '@angular/common';

@Component({
    selector: 'sh-user-panel',
    templateUrl: 'user-panel.component.html',
    styleUrls: ['./user-panel.component.scss'],
    imports: [NgIf, DxDropDownButtonModule, DxListModule, FontAwesomeModule]
})

export class UserPanelComponent {
  fasCircleUser = icon(faCircleUser).html[0];

  @Input() menuItems: any;

  @Input() menuMode = 'context';
}
