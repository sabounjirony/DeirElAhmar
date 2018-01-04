import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { DataTableModule, SharedModule } from 'primeng/primeng';

import { SystemRouting } from './system.routing';

import { CodeListComponent } from './../system/code/code-list/code-list.component';
import { CodeDetailComponent } from './../system/code/code-detail/code-detail.component';

import { DescriptionListComponent } from './../system/description/description-list/description-list.component';
import { DescriptionDetailComponent } from './../system/description/description-detail/description-detail.component';

import { ErrorListComponent } from './../system/log/errorlog/error-list/error-list.component';
import { ErrorDetailComponent } from './../system/log/errorlog/error-detail/error-detail.component';

import { EventListComponent } from './../system/log/eventlog/event-list/event-list.component';
import { EventDetailComponent } from './../system/log/eventlog/event-detail/event-detail.component';

import { MenuService } from './../system/security/menu/menu.service';
import { MenuListComponent } from './../system/security/menu/menu-list/menu-list.component';
import { MenuDetailComponent } from './../system/security/menu/menu-detail/menu-detail.component';

import { ModuleListComponent } from './../system/security/module/module-list/module-list.component';
import { ModuleDetailComponent } from './../system/security/module/module-detail/module-detail.component';
import { PermissionDetailComponent } from './../system/security/permission/permission-detail/permission-detail.component';

import { RoleListComponent } from './../system/security/role/role-list/role-list.component';
import { RoleDetailComponent } from './../system/security/role/role-detail/role-detail.component';

import { UserListComponent } from './../system/security/user/user-list/user-list.component';
import { UserDetailComponent } from './../system/security/user/user-detail/user-detail.component';

import { CodeRouting } from './code/code-routing';

@NgModule({
  declarations: [
    CodeListComponent, CodeDetailComponent,
    DescriptionListComponent, DescriptionDetailComponent,
    ErrorListComponent, ErrorDetailComponent,
    EventListComponent, EventDetailComponent,
    MenuListComponent, MenuDetailComponent,
    ModuleListComponent, ModuleDetailComponent,
    PermissionDetailComponent,
    RoleListComponent, RoleDetailComponent,
    UserListComponent, UserDetailComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    DataTableModule,
    SharedModule,
    CodeRouting,
    SystemRouting
  ]
})

export class SystemModule { }