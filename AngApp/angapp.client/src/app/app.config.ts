import { provideRouter } from '@angular/router';
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient,withInterceptorsFromDi, HTTP_INTERCEPTORS } from '@angular/common/http';
import { routes } from './app.routes';
import { CredentialsInterceptor} from './interceptors/credentialsInterceptor'

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClient(),
    //{
    //  provide: HTTP_INTERCEPTORS,
    //  useClass: CredentialsInterceptor,
    //  multi: true,
    //},
  ]
};
