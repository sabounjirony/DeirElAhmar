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
import { DatabaseComponent } from './database/database.component';

const routes: Routes = [
  {
    path: '', data: { title: 'System' },
    children: [
      { path: 'codes', component: CodeListComponent, data: { title: 'Codes' } },
      { path: 'errors', component: ErrorListComponent, data: { title: 'Errors' }  },
      { path: 'events', component: EventListComponent, data: { title: 'Events' }  },
      { path: 'descriptions', component: DescriptionListComponent, data: { title: 'Descrptions' }  },
      { path: 'menus', component: MenuListComponent, data: { title: 'Menus' }  },
      { path: 'modules', component: ModuleListComponent, data: { title: 'Modules' }  },
      { path: 'roles', component: RoleListComponent, data: { title: 'Roles' }  },
      { path: 'users/:id', component: UserDetailComponent },
      { path: 'users', component: UserListComponent, data: { title: 'Users' }  },
      { path: 'database', component: DatabaseComponent, data: { title: 'Database' }  }
    ]
  }
];

// const routes: Routes = [
//         { path: 'codes', component: CodeListComponent, children: [{ path: ':mode/:id', component: CodeDetailComponent }]},
//         { path: 'errors', component: ErrorListComponent },
//         { path: 'events', component: EventListComponent },
//         { path: 'descriptions', component: DescriptionListComponent },
//         { path: 'menus', component: MenuListComponent },
//         { path: 'modules', component: ModuleListComponent },
//         { path: 'roles', component: RoleListComponent },
//         { path: 'users/:id', component: UserDetailComponent },
//         { path: 'users', component: UserListComponent }
// ];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class SystemRouting { }