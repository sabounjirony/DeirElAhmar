import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { TestComponent } from './test/test.component';

const businessRoutes: Routes = [
  { path: "test", component: TestComponent }
];

@NgModule({
  imports: [RouterModule.forChild(businessRoutes)],
  exports: [RouterModule]
})

export class BusinessRouting { }