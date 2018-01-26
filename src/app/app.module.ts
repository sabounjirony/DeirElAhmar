import { NgModule, ViewContainerRef } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser'; // ng-If and ng-For
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';

import { AngularFireModule } from 'angularfire2';
import { AngularFireAuthModule } from 'angularfire2/auth';

import { codeReducer } from './system/code/codeReducer'
import { codeEffects } from './system/code/codeEffects'

import { userReducer } from './system/security/user/userReducer'
import { userEffects } from './system/security/user/userEffects'

import { HttpClientModule, HttpClient } from '@angular/common/http';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import { BlockUIModule, GrowlModule, MenubarModule, MenuItem, InputMaskModule, DataTableModule, SharedModule, ButtonModule } from 'primeng/primeng';

import { AppRoutingModule } from './app.routing.module';
import { AppFunctions } from './app.functions';

import { AppComponent } from './app.component';
import { LoginComponent } from './shared/login/login.component';
import { LogoutComponent } from './shared/logout/logout.component';
import { HeaderComponent } from './shared/header/header/header.component';
import { HomeComponent } from './shared/home/home.component';
import { ErrorComponent } from './shared/error/error.component';
import { FooterComponent } from './shared/footer/footer/footer.component';

import { UserService } from './system/security/user/user.service';
import { CodeService } from './system/code/codeService';
import { DescriptionService } from './system/description/description.service';
import { ErrorService } from './system/log/errorlog/error.service';
import { EventService } from './system/log/eventlog/event.service';
import { MenuService } from './system/security/menu/menu.service';
import { ModuleService } from './system/security/module/module.service';
import { PermissionService } from './system/security/permission/permission.service';
import { RoleService } from './system/security/role/role.service';

export const firebaseConfig = {
  apiKey: "AIzaSyA0BcUcu4V8aHT_gM-32BhRcmqji4z-lts",
  authDomain: "final-project-recording.firebaseapp.com",
  databaseURL: "https://final-project-recording.firebaseio.com",
  storageBucket: "final-project-recording.appspot.com",
  messagingSenderId: "290354329688"
};

@NgModule({
  //Custom Components directives and pipes
  declarations: [
    AppComponent,
    HeaderComponent,
    HomeComponent,
    ErrorComponent,
    FooterComponent,
    LoginComponent,
    LogoutComponent
  ],
  //Import third party finished modules, and custom modules only available for custom components defined above
  //The imports section is not distributed to the rest of the application need to re-import each time needed
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    HttpClientModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    BlockUIModule,
    GrowlModule,
    MenubarModule,
    ButtonModule,
    InputMaskModule,
    DataTableModule,
    SharedModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
    // AngularFireModule.initializeApp(firebaseConfig),
    // AngularFireAuthModule,
    // EffectsModule.forRoot([
    //   userEffects, codeEffects
    // ]),
    // StoreModule.forRoot([
    //   { state: codeReducer }
    // ])
  ],
  //Application scope
  providers: [AppFunctions, CodeService, DescriptionService, ErrorService, EventService, MenuService, ModuleService, PermissionService, RoleService, UserService],  //Services, can be added per component
  //Component to bootstrap application with, always AppComponent
  bootstrap: [AppComponent]
})

export class AppModule { }

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}