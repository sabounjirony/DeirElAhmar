import { Routes, RouterModule } from '@angular/router';

import { DescriptionListComponent } from './description-list/description-list.component';
import { DescriptionDetailComponent } from './description-detail/description-detail.component';

// const codeRoutes: Routes = [
//     { path: 'list', component: CodeListComponent },
//     { path: 'view/:id', component: CodeDetailComponent },
//     { path: 'add', component: CodeDetailComponent },
//     { path: 'edit/:id', component: CodeDetailComponent },
// ];

 const CODE_ROUTES: Routes = [
     { path: '', redirectTo: '/codes', pathMatch: 'full' },
     { path: 'codes/:mode/:id', component: DescriptionDetailComponent},
     { path: 'codes', component: DescriptionListComponent, children: [
        { path: ':mode/:id', component: DescriptionDetailComponent}
     ]},
     { path: 'list', component: DescriptionListComponent },
     { path: 'view/:id', component: DescriptionDetailComponent },
     { path: 'add', component: DescriptionDetailComponent },
     { path: 'edit/:id', component: DescriptionDetailComponent }];

export const CodeRouting = RouterModule.forChild(CODE_ROUTES);
