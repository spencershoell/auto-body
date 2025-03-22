import { Component, ComponentRef, Input, NgModule } from '@angular/core';

import DataSource from 'devextreme/data/data_source';

import { DxButtonModule, DxDataGridModule, DxDropDownButtonModule } from 'devextreme-angular';
import { RouterModule } from '@angular/router';
import { RowClickEvent } from 'devextreme/ui/data_grid';
import { SlidePanelService } from 'src/app/layouts';
import { BaseListComponent } from '../../_core';
import { Context, UserDataGrid } from 'src/app/data';
import { AuthorizationService, DataEventService } from 'src/app/shared/services';
import { takeUntil } from "rxjs";
import { UserCreateComponent } from './create.component';
import { UserSlideComponent } from './slide.component';
import { EntityType } from 'src/app/classes';
import { NgIf } from '@angular/common';
import { icon } from '@fortawesome/fontawesome-svg-core';
import { faPlus } from '@fortawesome/pro-solid-svg-icons';
import { faRefresh } from '@fortawesome/pro-light-svg-icons';

@Component({
    selector: 'sh-user-list',
    templateUrl: 'list.component.html',
    styleUrls: ['list.component.scss'],
    imports: [NgIf, RouterModule, DxButtonModule, DxDataGridModule, DxDropDownButtonModule]
})
export class UserListComponent extends BaseListComponent<UserDataGrid> {
  override entityName: string = "User";
  override entityType: string = EntityType.User;
  override pluralizedEntityName: string = "Users";

  @Input() outsideSlidePanel = false;

  fasPlus = icon(faPlus).html[0];
  falRefresh = icon(faRefresh).html[0];

  constructor(
    protected dataEventService: DataEventService,
    override authorizationService: AuthorizationService,
    override slidePanelService: SlidePanelService,
    context: Context
  ) {
    super();
    this._dataGrid = new UserDataGrid();
    this.dataSource = new DataSource({
      store: context.users,
      select: this.buildSelect(),
      filter: this.buildFilter(),
      onLoadingChanged: this.onLoadingChanged.bind(this)
    });
  }

  override async ngOnInit(): Promise<void> {
    await super.ngOnInit();

    this.dataEventService.userChanged$
      .pipe(takeUntil(this._destroying$))
      .subscribe(async () => { await this.refresh(); });
  }

  override create() {
    this.slidePanelService.pushComponent({
      componentType: UserCreateComponent,
      setInputs: () => { },
      forceOverlay: true,
      title: `Create ${this.entityName}`
    });
  }

  override onRowClick(e: RowClickEvent) {
    if (e.rowType !== 'data') return;

    const { data } = e;

    this.slidePanelService.pushComponent({
      componentType: UserSlideComponent,
      setInputs: (componentRef: ComponentRef<UserSlideComponent>) => {
        componentRef.instance.id = data.id;
      },
      clearSlidePanelBeforePush: this.outsideSlidePanel,
      title: `${this.entityName} Details`
    });
  }

  protected prepareFilter(filter: any[]): void { }
  protected prepareSelect(filter: any[]): void { }
}
