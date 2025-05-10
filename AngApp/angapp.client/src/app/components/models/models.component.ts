import { Component, inject, OnInit } from '@angular/core';
import { User } from '../../models/user.model';
//import Keycloak from 'keycloak-js';
import { HasRolesEnabledDirective } from '../../directives/has-roles-enabled.directive';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

@Component({
    selector: 'app-models',
    imports: [HasRolesEnabledDirective],
    templateUrl: 'models.component.html',
    styleUrls: [`models.component.scss`],
    providers: [HttpClient]
})
export class ModelsComponent {
    user: User | undefined;

    roles: string[] = [];

    modelResponse: string = '';

    private httpClient = inject(HttpClient);
    
    private getServerErrorMessage(error: HttpErrorResponse): string {
        switch (error.status) {
            case 404: {
                return `Not Found: ${error.message}`;
            }
            case 403: {
                return `Access Denied: ${error.message}`;
            }
            case 500: {
                return `Internal Server Error: ${error.message}`;
            }
            default: {
                return `Unknown Server Error: ${error.message}`;
            }

        }
    }


    private handleError(error: any) {
        if (error) {
            this.modelResponse = error.statusText;
            if (error.error instanceof ErrorEvent) {
                this.modelResponse = `Error: ${error.error.message}`;
            } else {
                this.modelResponse = this.getServerErrorMessage(error);
            }
        }
        return throwError(() => new Error('Something bad happened; please try again later.'));
    }


    RunModel(modelNumber: number) {
        this.httpClient.get(`externalapi/model${modelNumber}`).pipe(
            catchError(err => {
                return this.handleError(err)
            })
        ).subscribe((response) => {
            this.modelResponse = response.toString();
        });      
    }
}


