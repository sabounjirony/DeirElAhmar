import { NgModule }             from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
 
import { LoginComponent } from './shared/login/login.component';
import { LogoutComponent } from './shared/logout/logout.component';
import { HomeComponent } from './shared/home/home.component';
import { ErrorComponent } from './shared/error/error.component';

const routes: Routes = [
<<<<<<< HEAD
    { path: '', redirectTo: '/login', pathMatch: 'full' },
=======
<<<<<<< HEAD
    // { path: '', redirectTo: '/login', pathMatch: 'full' },
=======
    { path: '', redirectTo: '/login', pathMatch: 'full' },
>>>>>>> 
>>>>>>> 077bc3e4e0e76729b8f11737b895a8737d8e5ff2
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