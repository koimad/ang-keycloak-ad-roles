import { ActivatedRouteSnapshot, CanActivateFn,CanActivateChildFn, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { inject } from '@angular/core';
import { createAuthGuard } from './createAuthGuard';
import { AuthGuardData } from './AuthGuardData';
 const isAccessAllowed = async (
  route: ActivatedRouteSnapshot,
  __: RouterStateSnapshot,
  authData:AuthGuardData
 ): Promise<boolean | UrlTree> => {
  
  const { authenticated, grantedRoles } =  authData;

  const requiredRole = route.data['role'];
  if (!requiredRole) {
    return false;
  }

  const hasRequiredRole = (role: string): boolean => {   
    return Object.values(grantedRoles).some((roles) => roles.includes(role));
  }

  if (authenticated && hasRequiredRole(requiredRole)) {
    return true;
  }

     const router = inject(Router);
     return router.parseUrl(`/forbidden?destination=${route.url}`);
 };

 export const canActivateAuthRole = createAuthGuard<CanActivateFn>(isAccessAllowed); 
 
