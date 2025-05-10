import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../services/AuthService';

@Component({
  selector: 'app-forbidden',
  imports: [ 
    MatCardModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './forbidden.component.html',
  styleUrl: './forbidden.component.scss'
})
export class ForbiddenComponent {
constructor(private authService: AuthService) {}

  login(): void {
    this.authService.login();
  }
}
