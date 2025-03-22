import { Component } from '@angular/core';
import { icon } from '@fortawesome/fontawesome-svg-core';
import { faMoon, faSunBright } from '@fortawesome/pro-light-svg-icons';
import { DxButtonModule } from 'devextreme-angular';
import { ThemeService } from 'src/app/shared/services';

@Component({
    selector: 'sh-theme-switcher',
    template: `
      <dx-button
        class="theme-button"
        stylingMode="text"
        [icon]="themeService.currentTheme === 'dark' ? falSunBright : falMoon"
        (onClick)="onButtonClick()"
      ></dx-button>`,
    styleUrls: [],
    imports: [DxButtonModule]
})
export class ThemeSwitcherComponent {
  falMoon = icon(faMoon).html[0];
  falSunBright = icon(faSunBright).html[0];

  constructor(public themeService: ThemeService) { }

  onButtonClick() {
    this.themeService.switchTheme();
  }
}
