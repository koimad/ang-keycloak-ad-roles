import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { UserProfileComponent } from './components/user-profile/user-profile.component';
import { ModelsComponent } from './components/models/models.component';
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
    component: ModelsComponent,
    canActivate: [canActivateAuthRole],
    data: { role: 'models-user' }
  },
  {
    path: 'profile',
    component: UserProfileComponent,
    canActivate: [canActivateAuthRole],
    data: { role: 'aspire-editor' }
  },
  { path: 'forbidden', component: ForbiddenComponent },
  {
    path: 'signout-callback-oidc',
    redirectTo: 'home'
  }
];

