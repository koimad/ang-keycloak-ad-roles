import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { canActivateAuthRole } from './guards/auth-role.guard';
import { ForbiddenComponent } from './components/forbidden/forbidden.component';

export const routes: Routes = [
  { 
    path: '', 
    component: HomeComponent,    
  },
  {
    path: 'home', 
    component: HomeComponent
  },
  {
    path: 'models',
    canActivate: [canActivateAuthRole],
    loadChildren: () =>  import('../app/modules/models.module').then(m=> m.routes),
    data: { role: 'models-user' }
  },
  {
    path: 'profile',
    canActivate: [canActivateAuthRole],
    loadChildren: () =>  import('../app/modules/user-profile.module').then(m=> m.routes),
    data: { role: 'aspire-editor' }
  },
  { 
    path: 'forbidden', 
    loadChildren: () =>  import('../app/modules/forbidden.module').then(m=> m.routes)
   },
  {
    path: 'signout-callback-oidc',
    redirectTo: 'home'
  }
];

