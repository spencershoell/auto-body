import "reflect-metadata";
import themes from 'devextreme/ui/themes';
import 'zone.js';

import { importProvidersFrom } from '@angular/core';
import { AppComponent } from './app/root.component';
import { AppRoutingModule } from './app/root-routing.module';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { ScreenService, AppInfoService } from './app/shared/services';
import { HTTP_INTERCEPTORS, provideHttpClient, withFetch, withInterceptorsFromDi } from '@angular/common/http';
import { MSAL_GUARD_CONFIG, MSAL_INSTANCE, MSAL_INTERCEPTOR_CONFIG, MsalBroadcastService, MsalGuard, MsalInterceptor, MsalService } from '@azure/msal-angular';
import { MSALGuardConfigFactory, MSALInstanceFactory, MSALInterceptorConfigFactory } from './app/auth-config';

import config from 'devextreme/core/config';
import { licenseKey } from "./devextreme-license";

config({ licenseKey });

themes.initialized(() => {
    bootstrapApplication(AppComponent, {
        providers: [
            importProvidersFrom(BrowserModule, AppRoutingModule),
            provideHttpClient(withInterceptorsFromDi(), withFetch()),
            ScreenService,
            AppInfoService,
            {
                provide: HTTP_INTERCEPTORS,
                useClass: MsalInterceptor,
                multi: true
            },
            {
                provide: MSAL_INSTANCE,
                useFactory: MSALInstanceFactory
            },
            {
                provide: MSAL_GUARD_CONFIG,
                useFactory: MSALGuardConfigFactory
            },
            {
                provide: MSAL_INTERCEPTOR_CONFIG,
                useFactory: MSALInterceptorConfigFactory
            },
            MsalService,
            MsalGuard,
            MsalBroadcastService
        ]
    })
        .catch((err: any) => console.error(err));
});
