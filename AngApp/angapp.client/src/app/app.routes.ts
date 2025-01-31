import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { UserProfileComponent } from './components/user-profile/user-profile.component';

import { canActivateAuthRole } from './guards/auth-role.guard';
import { ModelsComponent } from './components/models/models.component';

export const routes: Routes = [

  { path: '', component: HomeComponent },
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
  //{ path: 'forbidden', component: ForbiddenComponent },
  //{ path: '**', component: NotFoundComponent }

];


