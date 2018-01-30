import { NgModule }             from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
 
import { LoginComponent } from './shared/login/login.component';
import { LogoutComponent } from './shared/logout/logout.component';
import { HomeComponent } from './shared/home/home.component';
import { ErrorComponent } from './shared/error/error.component';

const routes: Routes = [
    // { path: '', redirectTo: '/login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'logout', component: LogoutComponent },
    { path: 'error', component: ErrorComponent },
    //{ path: '**', redirectTo: "error", pathMatch: "full" }, //Fail over any route redirect sample
    { path: 'home', component: HomeComponent, children: [
      { path: 'business', loadChildren: 'app/business/business.module#BusinessModule' },
      { path: 'setup', loadChildren: 'app/setup/setup.module#SetupModule' },
      { path: 'system', loadChildren: 'app/system/system.module#SystemModule' },
    ] }
  ];

  @NgModule({
    imports: [ RouterModule.forRoot(routes) ],
    exports: [ RouterModule ]
  })

  export class AppRoutingModule {}