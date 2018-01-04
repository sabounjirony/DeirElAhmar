import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';

import { SetupRouting } from './setup.routing';

import { DataTableModule, SharedModule } from 'primeng/primeng';
import { ClientComponent } from './client/client/client.component';

@NgModule({
  declarations: [
  ClientComponent
],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    SetupRouting
  ]
})

export class SetupModule { }