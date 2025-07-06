import { Routes } from '@angular/router';
import { ModelsComponent } from '../components/models/models.component';
import { canActivateAuthRole } from '../guards/auth-role.guard';

export const routes: Routes = [
  {
    path: '',
    component: ModelsComponent,
    canActivate: [canActivateAuthRole],
    data: { role: 'models-user' }
  },

];