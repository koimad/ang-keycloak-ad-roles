import { Routes } from '@angular/router';
import { canActivateAuthRole } from '../guards/auth-role.guard';
import { UserProfileComponent } from '../components/user-profile/user-profile.component';

export const routes: Routes = [
  {
    path: '',
    component: UserProfileComponent,  
    canActivate: [canActivateAuthRole],  
    data: { role: 'aspire-editor' }
  },

];