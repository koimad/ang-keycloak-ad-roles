import { AuthService } from "../services/AuthService";

export type AuthGuardData = {
    authenticated: boolean;
    grantedRoles: string[];
    authService: AuthService;
};
