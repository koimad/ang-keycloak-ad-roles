import { Component, effect, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthService} from '../../services/AuthService'

import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {MatToolbarModule} from '@angular/material/toolbar';

@Component({
  selector: 'app-menu',
  imports: [RouterModule, MatToolbarModule, MatButtonModule, MatIconModule],
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent  {
  authenticated = false;
  keycloakStatus: string | undefined;
  
  private readonly authService = inject(AuthService) ;

  constructor() {
    effect(() => {
      this.authenticated = this.authService.authStateChanged();   
      
      if(this.authenticated)
      {
        this.keycloakStatus = "Logged In";
      }
      else{
        this.keycloakStatus = "Logged Out";
      }
      
    });
  }
     
  login() {
    this.authService.login();          
  }

  logout() {
    this.authService.logout();    
  }
}
