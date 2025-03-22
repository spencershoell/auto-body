import { Component, ComponentRef, Input } from '@angular/core';
import DataSource from 'devextreme/data/data_source';
import { DxButtonModule, DxDataGridModule, DxDropDownButtonModule } from 'devextreme-angular';
import { RouterModule } from '@angular/router';
import { RowClickEvent } from 'devextreme/ui/data_grid';
import { SlidePanelService } from 'src/app/layouts/slide-panel/slide-panel.services';
import { BaseListComponent } from '../../_core';
import { GroupDataGrid } from 'src/app/data/models';
import { AuthorizationService, DataEventService } from 'src/app/shared/services';
import { takeUntil } from "rxjs";
import { GroupCreateComponent } from './create.component';
import { GroupSlideComponent } from './slide.component';
import { EntityType } from 'src/app/classes';
import { NgIf } from '@angular/common';
import { Context } from 'src/app/data/context';
import { icon } from '@fortawesome/fontawesome-svg-core';
import { faPlus } from '@fortawesome/pro-solid-svg-icons';
import { faRefresh } from '@fortawesome/pro-light-svg-icons';

@Component({
    selector: 'sh-group-list',
    templateUrl: 'list.component.html',
    styleUrls: ['list.component.scss'],
    imports: [NgIf, RouterModule, DxButtonModule, DxDataGridModule, DxDropDownButtonModule]
})
export class GroupListComponent extends BaseListComponent<GroupDataGrid> {
  override entityName: string = "Group";
  override entityType: string = EntityType.Group;
  override pluralizedEntityName: string = "Groups";

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
    this._dataGrid = new GroupDataGrid();
    this.dataSource = new DataSource({
      store: context.groups,
      select: this.buildSelect(),
      filter: this.buildFilter(),
      onLoadingChanged: this.onLoadingChanged.bind(this)
    });
  }

  public override async ngOnInit(): Promise<void> {
    await super.ngOnInit();

    this.dataEventService.groupChanged$
      .pipe(takeUntil(this._destroying$))
      .subscribe(async () => { await this.refresh(); });
  }

  override create() {
    this.slidePanelService.pushComponent({
      componentType: GroupCreateComponent,
      setInputs: () => { },
      forceOverlay: true,
      title: `Create ${this.entityName}`
    });
  }

  override onRowClick(e: RowClickEvent) {
    if (e.rowType !== 'data') return;

    const { data } = e;

    this.slidePanelService.pushComponent({
      componentType: GroupSlideComponent,
      setInputs: (componentRef: ComponentRef<GroupSlideComponent>) => {
        componentRef.instance.id = data.id;
      },
      clearSlidePanelBeforePush: this.outsideSlidePanel,
      title: `${this.entityName} Details`
    });
  }

  override prepareFilter(filter: any[]): void { }
  override prepareSelect(filter: any[]): void { }
}