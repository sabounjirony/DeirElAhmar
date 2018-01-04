import { Routes, RouterModule } from '@angular/router';

import { CodeListComponent } from './code-list/code-list.component';
import { CodeDetailComponent } from './code-detail/code-detail.component';

// const codeRoutes: Routes = [
//     { path: 'list', component: CodeListComponent },
//     { path: 'view/:id', component: CodeDetailComponent },
//     { path: 'add', component: CodeDetailComponent },
//     { path: 'edit/:id', component: CodeDetailComponent },
// ];

 const CODE_ROUTES: Routes = [
     { path: '', redirectTo: '/codes', pathMatch: 'full' },
     { path: 'codes/:mode/:id', component: CodeDetailComponent},
     { path: 'codes', component: CodeListComponent, children: [
        { path: ':mode/:id', component: CodeDetailComponent}
     ]},
     { path: 'list', component: CodeListComponent },
     { path: 'view/:id', component: CodeDetailComponent },
     { path: 'add', component: CodeDetailComponent },
     { path: 'edit/:id', component: CodeDetailComponent }];

export const CodeRouting = RouterModule.forChild(CODE_ROUTES);
