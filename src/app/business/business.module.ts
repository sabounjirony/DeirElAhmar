import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';

import { BusinessRouting } from './business.routing';

import { DataTableModule, SharedModule } from 'primeng/primeng';
import { TestComponent } from './test/test.component';


@NgModule({
  declarations: [
  TestComponent
],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    BusinessRouting
  ],
  providers: [],
})

export class BusinessModule { }