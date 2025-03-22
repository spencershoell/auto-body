import { Component } from '@angular/core';

import { DxToolbarModule } from 'devextreme-angular/ui/toolbar';
import { DxLoadIndicatorModule } from 'devextreme-angular';

@Component({
    selector: 'sh-not-authenticated',
    templateUrl: 'not-authenticated.component.html',
    styleUrls: ['./not-authenticated.component.scss'],
    imports: [DxLoadIndicatorModule, DxToolbarModule]
})
export class NotAuthentcatedComponent {
}
