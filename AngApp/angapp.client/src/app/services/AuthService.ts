import { HttpClient } from "@angular/common/http";
import { Injectable, signal } from "@angular/core";
import { User } from "../models/user.model";

@Injectable({ providedIn: 'root' })
export class AuthService {

    public authStateChanged = signal<boolean>(false);

    public isLoggedIn = false;
    
    public user: User | null = null;

    constructor(private http: HttpClient) {
        this.refreshSessionStatus();
    }

    public login(): void {
        location.href = '/login';
        this.markLoggedIn();
        this.refreshSessionStatus();
    }

    public logout(): void {
        location.href = '/logout';
        this.markLoggedOut();
    }


    public isInRole(role: string): boolean{
        let result = false;
        
        if(this.user && this.user.userRoles.includes(role)) {
            result = true;
        }
        return result;
    }

    public grantedRoles() : string[] {
        let result : string[] = [];

        if (this.user){
            result = this.user.userRoles;
        }
        return result;
    }

    private markLoggedIn(): void {
        this.isLoggedIn = true;
        this.authStateChanged.set(true);
    }

    private markLoggedOut(): void {
        this.isLoggedIn = false;
        this.user = null;
        this.authStateChanged.set(false);
        
    }

    private refreshSessionStatus(): void {
       
       if(!this.user)
       {
        // Check if we have a valid session
        this.http.get<User>(`/userinfo` ) 
            .subscribe({
                next: (user) => {
                    this.user = user;
                    this.markLoggedIn();
                },
                error: (_err) => {
                    this.user = null;
                    this.markLoggedOut();
                }
            });
        }
    }
}







