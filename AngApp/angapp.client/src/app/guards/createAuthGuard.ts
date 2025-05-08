import { ActivatedRouteSnapshot, CanActivateChildFn, CanActivateFn, RouterStateSnapshot, UrlTree } from "@angular/router";
import { AuthService } from "../services/AuthService";
import { inject } from "@angular/core";
import { AuthGuardData } from "./AuthGuardData";

export const createAuthGuard = <T extends CanActivateFn | CanActivateChildFn>(
    isAccessAllowed: (
      route: ActivatedRouteSnapshot,
      state: RouterStateSnapshot,
      authData: AuthGuardData
    ) => Promise<boolean | UrlTree>
  ): T => {
    return ((next: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
      const authService = inject(AuthService); 
      
      const authenticated : boolean = authService?.isLoggedIn ?? false;
      const grantedRoles : string [] = authService?.grantedRoles() ?? []; 
      const authData = { authenticated, authService, grantedRoles };
  
      return isAccessAllowed(next, state, authData);
    }) as T;
  };