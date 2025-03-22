import { Component } from '@angular/core';
import { DxButtonModule } from 'devextreme-angular/ui/button';
import { DxToolbarModule } from 'devextreme-angular/ui/toolbar';
import { MsalService } from '@azure/msal-angular';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faRightFromBracket } from '@fortawesome/pro-duotone-svg-icons';
import { JsonPipe } from '@angular/common';

@Component({
    selector: 'sh-not-authorized',
    templateUrl: 'not-authorized.component.html',
    styleUrls: ['./not-authorized.component.scss'],
    imports: [JsonPipe, DxButtonModule, DxToolbarModule, FontAwesomeModule]
})
export class NotAuthorizedComponent {
  faRightFromBracket = faRightFromBracket;
  account: any;

  constructor(private authService: MsalService) {
    this.account = authService.instance.getAllAccounts()[0];
  }

  logout(e: Event) {
    e.preventDefault();
    this.authService.logout();
  }
}
