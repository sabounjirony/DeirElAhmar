import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ClientComponent } from './client/client/client.component';

const setupRoutes: Routes = [ 
    { path: 'client', component: ClientComponent }
];

@NgModule({
    imports: [RouterModule.forChild(setupRoutes)],
    exports: [RouterModule]
  })
  
  export class SetupRouting { }