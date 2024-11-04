import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { routes } from './app/app.routes';
import { AppComponent } from './app/app.component';
import { provideRouter } from '@angular/router';
import { importProvidersFrom } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { registerLicense } from '@syncfusion/ej2-base';

  bootstrapApplication(AppComponent, {
    providers: [
      provideRouter(routes),
      importProvidersFrom(HttpClientModule, BrowserAnimationsModule)
    ]
  }).catch(err => console.error(err));

  registerLicense('Ngo9BigBOggjHTQxAR8/V1NDaF5cWWtCf1JpRGNGfV5ycEVCal9WTnVWUj0eQnxTdEFiWH9dcXJWQWNVUkZzVg=='); //SYNCF_LICENSE_KEY
