import { Component, OnInit } from '@angular/core';
import { User } from '../../models/user.model';
import Keycloak from 'keycloak-js';
import { HasRolesEnabledDirective } from '../../directives/has-roles-enabled.directive';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

@Component({
  selector: 'app-models',
  imports: [HasRolesEnabledDirective],
  templateUrl: 'models.component.html',
  styleUrls: [`models.component.css`],
  providers: [HttpClient]
 
})
export class ModelsComponent implements OnInit {
  user: User | undefined;

  roles: string[] = [];

  modelResponse: string = '';

  constructor(private readonly keycloak: Keycloak, private httpClient: HttpClient) { }

  async ngOnInit() {
    if (this.keycloak?.authenticated) {
      const profile = await this.keycloak.loadUserProfile();
         
      this.user = { name: `${profile?.firstName} ${profile.lastName}`   
      };
    }
  }

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

  handleError(error : any) {
    console.log(error.statusText);

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
    console.log(`Model ${modelNumber} is running`);

    this.httpClient.get(`https://localhost:7052/externalapi/model${modelNumber}`).pipe(
      catchError(err => {
        return this.handleError(err)
      })
    ).subscribe((response) => {
      this.modelResponse = response.toString();
    });    
  }
}
