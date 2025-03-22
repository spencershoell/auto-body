import { Component, Input, Output, EventEmitter } from '@angular/core';
import { NgIf } from '@angular/common';

import { DxButtonModule } from 'devextreme-angular/ui/button';
import { DxToolbarModule } from 'devextreme-angular/ui/toolbar';

import { Router } from '@angular/router';
import { MsalService } from '@azure/msal-angular';
import { UserPanelComponent } from '../user-panel/user-panel.component';
import { ThemeSwitcherComponent } from '../theme-switcher/theme-switcher.component';
import { faRightFromBracket, faUser } from '@fortawesome/pro-regular-svg-icons';
import { icon } from '@fortawesome/fontawesome-svg-core';

@Component({
    selector: 'sh-header',
    templateUrl: 'header.component.html',
    styleUrls: ['./header.component.scss'],
    imports: [DxToolbarModule, NgIf, DxButtonModule, ThemeSwitcherComponent, UserPanelComponent]
})

export class HeaderComponent {
  @Output()
  menuToggle = new EventEmitter<boolean>();

  @Input()
  menuToggleEnabled = false;

  @Input()
  title!: string;

  farUser = icon(faUser).html[0];
  farRightFromBracket = icon(faRightFromBracket).html[0];

  userMenuItems = [{
    text: 'Profile',
    icon: this.farUser,
    onClick: () => {
      this.router.navigate(['/profile']);
    }
  },
  {
    text: 'Logout',
    icon: this.farRightFromBracket,
    onClick: () => {
      this.authService.logout();
    }
  }];

  constructor(
    private authService: MsalService,
    private router: Router
  ) { }

  toggleMenu = () => {
    this.menuToggle.emit();
  }
}
