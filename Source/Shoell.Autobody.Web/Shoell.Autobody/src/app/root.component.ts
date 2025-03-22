import { Component, HostBinding, OnDestroy, OnInit } from '@angular/core';
import { ScreenService, AppInfoService } from './shared/services';
import { RouterOutlet } from '@angular/router';
import { SideNavComponent, SlidePanelComponent } from './layouts';
import { NgIf } from '@angular/common';
import { Subject, filter, takeUntil, timeout } from 'rxjs';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { InteractionStatus } from '@azure/msal-browser';
import { NotAuthentcatedComponent, NotAuthorizedComponent } from './shared/components';
import { DxHttpModule } from 'devextreme-angular/http';

@Component({
  selector: 'sh-root',
  templateUrl: './root.component.html',
  styleUrls: ['./root.component.scss'],
  imports: [NgIf, DxHttpModule, SideNavComponent, RouterOutlet, SlidePanelComponent, NotAuthentcatedComponent, NotAuthorizedComponent]
})
export class AppComponent implements OnInit, OnDestroy {
  @HostBinding('class') get getClass() {
    return Object.keys(this.screen.sizes).filter(cl => this.screen.sizes[cl]).join(' ');
  }

  private readonly _destroying$ = new Subject<void>();

  loggedIn: boolean = false;
  authorized: boolean = false;

  constructor(
    private authService: MsalService,
    private screen: ScreenService,
    private msalBroadcastService: MsalBroadcastService,
    public appInfo: AppInfoService,
  ) { }

  async ngOnInit(): Promise<void> {
    this.msalBroadcastService.inProgress$
      .pipe(
        filter((status: InteractionStatus) => status === InteractionStatus.None),
        takeUntil(this._destroying$)
      )
      .subscribe(() => {
        this.checkAccount();
      });
  }

  ngOnDestroy(): void {
    this._destroying$.unsubscribe();
  }

  checkAccount() {
    this.loggedIn = !!this.authService.instance.getAllAccounts()[0];
    if (this.loggedIn) {
      this.authorized = true;
    }
  }
}
