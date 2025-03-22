import { Component, ComponentRef, Input, NgModule } from '@angular/core';

import DataSource from 'devextreme/data/data_source';
import Guid from 'devextreme/core/guid';
import { Common } from 'src/app/classes';
import { DxButtonModule, DxDataGridModule } from 'devextreme-angular';
import { RouterModule } from '@angular/router';
import { User_GroupDataGrid } from 'src/app/data/models';
import { SlidePanelService } from 'src/app/layouts/slide-panel/slide-panel.services';
import { User_GroupLinkComponent } from './link.component';
import { BaseJoinListComponent } from 'src/app/shared/components';
import { AuthorizationService } from 'src/app/shared/services';
import { Context } from 'src/app/data';
import { icon } from '@fortawesome/fontawesome-svg-core';
import { faLink, faPlus } from '@fortawesome/pro-solid-svg-icons';
import { faRefresh, faSave } from '@fortawesome/pro-light-svg-icons';
import { GroupCreateComponent } from '../../group/create.component';
import { UserCreateComponent } from '../../user/create.component';
import { NgIf } from '@angular/common';

@Component({
    selector: 'sh-user-group-list',
    templateUrl: 'list.component.html',
    styleUrls: ['list.component.scss'],
    imports: [NgIf, RouterModule, DxButtonModule, DxDataGridModule]
})
export class User_GroupListComponent extends BaseJoinListComponent<User_GroupDataGrid> {
  @Input() userId?: Guid;
  @Input() groupId?: Guid;

  get showUser(): boolean {
    return Common.isNull(this.userId);
  }

  get showGroup(): boolean {
    return Common.isNull(this.groupId);
  }

  get canCreateUser(): boolean {
    return !this.userId && this.authorizationService.canCreate(this.entityType1);
  }

  get canCreateGroup(): boolean {
    return !this.groupId && this.authorizationService.canCreate(this.entityType2);
  }

  override entityType1: string = "User";
  override entityType2: string = "Group";

  fasPlus = icon(faPlus).html[0];
  fasLink = icon(faLink).html[0];
  falRefresh = icon(faRefresh).html[0];


  constructor(
    private slideService: SlidePanelService,
    protected authorizationService: AuthorizationService,
    context: Context,
  ) {
    super();
    this._dataGrid = new User_GroupDataGrid();
    this.dataSource = new DataSource({
      store: context.user_Groups,
      select: this.buildSelect(),
      filter: this.buildFilter(),
      onLoadingChanged: this.onLoadingChanged.bind(this)
    });

    slideService.popComponent$.subscribe((componentType) => {
      if (componentType === User_GroupLinkComponent) {
        this.refresh();
      }
    });

    this.createGroup = this.createGroup.bind(this);
    this.createUser = this.createUser.bind(this);
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

  createUser(): void {
    this.slideService.pushComponent({
      componentType: UserCreateComponent,
      setInputs: (componentRef: ComponentRef<UserCreateComponent>) => {
      },
      title: `Create User`,
      forceOverlay: true
    });
  }

  link(): void {
    this.slideService.pushComponent({
      componentType: User_GroupLinkComponent,
      setInputs: (componentRef: ComponentRef<User_GroupLinkComponent>) => {
        if (Common.isNotNull(this.userId)) {
          componentRef.instance.userId = this.userId;
        }
        if (Common.isNotNull(this.groupId)) {
          componentRef.instance.groupId = this.groupId;
        }
      },
      title: `Link ${this.showUser ? "User" : "Group"} to ${!this.showUser ? "User" : "Group"}`
    });
  }

  protected prepareFilters(filter: any[]): void {
    if (Common.isNotNull(this.userId)) {
      filter.push(["userId", this.userId?.valueOf()]);
    }
    if (Common.isNotNull(this.groupId)) {
      if (filter.length > 0) {
        filter.push("and");
      }
      filter.push(["groupId", this.groupId?.valueOf()]);
    }
  }

  protected override buildSelect(): any[] {
    let select = [];
    select.push("userId", "user.name", "user.externalId");
    select.push("groupId", "group.name", "group.description");
    return select;
  }
}
