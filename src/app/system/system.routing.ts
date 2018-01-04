import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { CodeListComponent } from './code/code-list/code-list.component';
import { CodeDetailComponent } from './code/code-detail/code-detail.component';
import { DescriptionListComponent } from './description/description-list/description-list.component';
import { ErrorListComponent } from './log/errorlog/error-list/error-list.component';
import { EventListComponent } from './log/eventlog/event-list/event-list.component';
import { MenuListComponent } from './security/menu/menu-list/menu-list.component';
import { ModuleListComponent } from './security/module/module-list/module-list.component';
import { RoleListComponent } from './security/role/role-list/role-list.component';
import { UserListComponent } from './security/user/user-list/user-list.component';
import { UserDetailComponent } from './security/user/user-detail/user-detail.component';

const routes: Routes = [
  {
    path: 'codes', component: CodeListComponent, children: [
      { path: ':mode/:id', component: CodeDetailComponent }
    ]
  },
  { path: 'errors', component: ErrorListComponent },
  { path: 'events', component: EventListComponent },
  { path: 'descriptions', component: DescriptionListComponent },
  { path: 'menus', component: MenuListComponent },
  { path: 'modules', component: ModuleListComponent },
  { path: 'roles', component: RoleListComponent },
  { path: 'users/:id', component: UserDetailComponent },
  { path: 'users', component: UserListComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class SystemRouting { }