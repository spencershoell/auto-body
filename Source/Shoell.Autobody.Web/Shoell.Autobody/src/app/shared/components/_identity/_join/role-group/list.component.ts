import { Component, ComponentRef, Input, NgModule } from '@angular/core';

import DataSource from 'devextreme/data/data_source';
import Guid from 'devextreme/core/guid';
import { Common } from 'src/app/classes';
import { DxButtonModule, DxDataGridModule } from 'devextreme-angular';
import { RouterModule } from '@angular/router';
import { BaseJoinListComponent } from 'src/app/shared/components';
import { Role_GroupDataGrid } from 'src/app/data/models';
import { SlidePanelService } from 'src/app/layouts/slide-panel/slide-panel.services';
import { Role_GroupLinkComponent } from './link.component';
import { AuthorizationService } from 'src/app/shared/services';
import { Context } from 'src/app/data';
import { icon } from '@fortawesome/fontawesome-svg-core';
import { faLink, faPlus } from '@fortawesome/pro-solid-svg-icons';
import { GroupCreateComponent } from '../../group/create.component';
import { NgIf } from '@angular/common';
import { faRefresh } from '@fortawesome/pro-light-svg-icons';

@Component({
    selector: 'sh-role-group-list',
    templateUrl: 'list.component.html',
    styleUrls: ['list.component.scss'],
    imports: [NgIf, RouterModule, DxButtonModule, DxDataGridModule]
})
export class Role_GroupListComponent extends BaseJoinListComponent<Role_GroupDataGrid> {
  @Input() roleId?: Guid;
  @Input() groupId?: Guid;

  get showRole(): boolean {
    return Common.isNull(this.roleId);
  }

  get showGroup(): boolean {
    return Common.isNull(this.groupId);
  }

  get canCreateGroup(): boolean {
    return !this.groupId && this.authorizationService.canCreate(this.entityType2);
  }

  override entityType1: string = "Role";
  override entityType2: string = "Group";

  fasPlus = icon(faPlus).html[0];
  fasLink = icon(faLink).html[0];
  falRefresh = icon(faRefresh).html[0];

  constructor(
    private slideService: SlidePanelService,
    protected authorizationService: AuthorizationService,
    protected context: Context
  ) {
    super();
    this._dataGrid = new Role_GroupDataGrid();
    this.dataSource = new DataSource({
      store: context.role_Groups,
      select: this.buildSelect(),
      filter: this.buildFilter(),
      onLoadingChanged: this.onLoadingChanged.bind(this)
    });

    slideService.popComponent$.subscribe((componentType) => {
      if (componentType === Role_GroupLinkComponent) {
        this.refresh();
      }
    });

    this.createGroup = this.createGroup.bind(this);
  }

  createGroup(): void {
    this.slideService.pushComponent({
      componentType: GroupCreateComponent,
      setInputs: (componentRef: ComponentRef<GroupCreateComponent>) => {
      },
      title: `Create Group`,
      forceOverlay: true
    });
  }

  link(): void {
    this.slideService.pushComponent({
      componentType: Role_GroupLinkComponent,
      setInputs: (componentRef: ComponentRef<Role_GroupLinkComponent>) => {
        if (Common.isNotNull(this.roleId)) {
          componentRef.instance.roleId = this.roleId;
        }
        if (Common.isNotNull(this.groupId)) {
          componentRef.instance.groupId = this.groupId;
        }
      },
      title: `Link ${this.showRole ? "Role" : "Group"} to ${!this.showRole ? "Role" : "Group"}`
    });
  }

  protected prepareFilters(filter: any[]): void {
    if (Common.isNotNull(this.roleId)) {
      filter.push(["roleId", this.roleId?.valueOf()]);
    }
    if (Common.isNotNull(this.groupId)) {
      if (filter.length > 0) {
        filter.push("and");
      }
      filter.push(["groupId", this.groupId?.valueOf()]);
    }
  }

  protected override buildSelect(): any[] {
    let select = super.buildSelect();
    select.push("role.name", "role.target", "role.operation");
    select.push("group.name", "group.description");
    return select;
  }
}
