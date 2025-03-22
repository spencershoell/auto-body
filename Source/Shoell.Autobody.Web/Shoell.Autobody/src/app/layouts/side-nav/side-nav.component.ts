import { Component, OnInit, Input, ViewChild } from '@angular/core';

import { ScreenService } from 'src/app/shared/services';
import { DxTreeViewTypes } from 'devextreme-angular/ui/tree-view';
import { DxDrawerModule, DxDrawerTypes } from 'devextreme-angular/ui/drawer';

import { Router, NavigationEnd } from '@angular/router';
import { ThemeService } from 'src/app/shared/services/theme-service';
import { NgClass } from '@angular/common';
import { HeaderComponent, SideNavigationMenuComponent } from 'src/app/shared/components';

@Component({
    selector: 'sh-side-nav-outer-toolbar',
    templateUrl: './side-nav.component.html',
    styleUrls: ['./side-nav.component.scss'],
    imports: [NgClass, DxDrawerModule, HeaderComponent, SideNavigationMenuComponent]
})
export class SideNavComponent implements OnInit {
  selectedRoute = '';

  menuOpened!: boolean;
  temporaryMenuOpened = false;

  @Input() title!: string;

  menuMode: DxDrawerTypes.OpenedStateMode = 'shrink';
  menuRevealMode: DxDrawerTypes.RevealMode = 'expand';
  minMenuSize = 0;
  shaderEnabled = false;
  swatchClassName = 'dx-swatch-additional';

  constructor(
    private router: Router,
    private screen: ScreenService,
    protected themeService: ThemeService
  ) {
    themeService.isDark.subscribe((isDark) => {
      this.swatchClassName = 'dx-swatch-additional' + (isDark ? '-dark' : '');
    })
  }

  ngOnInit() {
    this.menuOpened = this.screen.sizes['screen-large'];

    this.router.events.subscribe(val => {
      if (val instanceof NavigationEnd) {
        this.selectedRoute = val.urlAfterRedirects.split('?')[0];
      }
    });

    this.screen.changed.subscribe(() => this.updateDrawer());

    this.updateDrawer();
  }

  updateDrawer() {
    const isXSmall = this.screen.sizes['screen-x-small'];
    const isLarge = this.screen.sizes['screen-large'];

    this.menuMode = isLarge ? 'shrink' : 'overlap';
    this.menuRevealMode = isXSmall ? 'slide' : 'expand';
    this.minMenuSize = isXSmall ? 0 : 60;
    this.shaderEnabled = !isLarge;
  }

  get hideMenuAfterNavigation() {
    return this.menuMode === 'overlap' || this.temporaryMenuOpened;
  }

  get showMenuAfterClick() {
    return !this.menuOpened;
  }

  navigationChanged(event: DxTreeViewTypes.ItemClickEvent) {
    const path = (event.itemData as any).path;
    const pointerEvent = event.event;

    if (path && this.menuOpened) {
      this.router.navigate([path]);

      if (this.hideMenuAfterNavigation) {
        this.temporaryMenuOpened = false;
        this.menuOpened = false;
        pointerEvent?.stopPropagation();
      }
    } else {
      pointerEvent?.preventDefault();
    }
  }

  navigationClick() {
    if (this.showMenuAfterClick) {
      this.temporaryMenuOpened = true;
      this.menuOpened = true;
    }
  }
}
