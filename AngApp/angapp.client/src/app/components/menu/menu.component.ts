import { Component, effect, inject, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
 import { AuthService} from '../../services/AuthService'

@Component({
  selector: 'app-menu',
  imports: [RouterModule],
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
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
