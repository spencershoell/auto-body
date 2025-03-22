import { Component, Output, Input, EventEmitter, ViewChild, ElementRef, AfterViewInit, OnDestroy } from '@angular/core';
import { DxTreeViewModule, DxTreeViewComponent, DxTreeViewTypes } from 'devextreme-angular/ui/tree-view';

import * as events from 'devextreme/events';
import { navigation } from 'src/app/root-navigation';

@Component({
    selector: 'sh-side-navigation-menu',
    templateUrl: './side-navigation-menu.component.html',
    styleUrls: ['./side-navigation-menu.component.scss'],
    imports: [DxTreeViewModule]
})
export class SideNavigationMenuComponent implements AfterViewInit, OnDestroy {
  private _compactMode = false;
  private _items!: Record<string, unknown>[];
  private _selectedItem!: String;

  @ViewChild(DxTreeViewComponent, { static: true }) menu!: DxTreeViewComponent;

  @Output() selectedItemChanged = new EventEmitter<DxTreeViewTypes.ItemClickEvent>();

  @Output() openMenu = new EventEmitter<any>();

  @Input() set selectedItem(value: String) {
    this._selectedItem = value;
    if (!this.menu.instance) {
      return;
    }

    this.menu.instance.selectItem(value);
  }

  get items() {
    if (!this._items) {
      this._items = navigation.map((item: any) => {
        if (item.path && !(/^\//.test(item.path))) {
          item.path = `/${item.path}`;
        }
        return { ...item, expanded: !this._compactMode }
      });
    }

    return this._items;
  }

  @Input() get compactMode() {
    return this._compactMode;
  }
  set compactMode(val) {
    this._compactMode = val;

    if (!this.menu.instance) {
      return;
    }

    if (val) {
      this.menu.instance.collapseAll();
    } else {
      this.menu.instance.expandItem(this._selectedItem);
    }
  }

  constructor(private elementRef: ElementRef) { }

  onItemClick(event: DxTreeViewTypes.ItemClickEvent) {
    this.selectedItemChanged.emit(event);
  }

  ngAfterViewInit() {
    events.on(this.elementRef.nativeElement, 'dxclick', (e: Event) => {
      this.openMenu.next(e);
    });
  }

  ngOnDestroy() {
    events.off(this.elementRef.nativeElement, 'dxclick');
  }
}
