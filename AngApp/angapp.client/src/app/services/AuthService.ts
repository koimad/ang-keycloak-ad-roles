import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Injectable, signal} from "@angular/core";
import {User} from "../models/user.model";
import { ActivatedRoute } from "@angular/router";

@Injectable({providedIn: 'root'})
export class AuthService {

    public authStateChanged = signal<boolean>(false);

    public isLoggedIn = false;

    public user: User | null = null;

    constructor(private http: HttpClient, private route: ActivatedRoute) {
        this.refreshSessionStatus();
    }

    public login(): void {

        let url = '/auth/login';
        let dest = this.route.snapshot.queryParamMap.get('destination');

        if (dest) {
            url = `/auth/login?redirectUrl=/${dest}`;
        }

        console.log(url);

        location.href = url;
              

        this.markLoggedIn();
        this.refreshSessionStatus();
    }

    public logout(): void {
        location.href = '/auth/logout';
        this.markLoggedOut();
    }


    public isInRole(role: string): boolean {
        let result = false;

        if (this.user && this.user.roles.includes(role)) {
            result = true;
        }
        return result;
    }

    public grantedRoles(): string[] {
        let result: string[] = [];

        if (this.user) {
            result = this.user.roles;
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
        if (!this.user) {            
            this.http.get<User>(`/auth/profile`)    
                .subscribe({
                    next: (user) => {
                        console.log(user);
                        this.user = user;
                        this.markLoggedIn();
                    },
                    error: (_err) => {
                        this.markLoggedOut();
                    }
                });
        }
    }
}







