import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';
import { UserListPageComponent } from './pages/_identity/user/list.page';
import { GroupListPageComponent } from './pages/_identity/group/list.page';
import { UserDetailPageComponent } from './pages/_identity/user/detail.page';
import { GroupDetailPageComponent } from './pages/_identity/group/detail.page';

const routes: Routes = [
  {
    path: 'user',
    component: UserListPageComponent,
    canActivate: [MsalGuard]
  },
  {
    path: 'user/:id',
    component: UserDetailPageComponent,
    canActivate: [MsalGuard]
  },
  {
    path: 'group',
    component: GroupListPageComponent,
    canActivate: [MsalGuard]
  },
  {
    path: 'group/:id',
    component: GroupDetailPageComponent,
    canActivate: [MsalGuard]
  },
  {
    path: '**',
    redirectTo: 'user'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: false })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
