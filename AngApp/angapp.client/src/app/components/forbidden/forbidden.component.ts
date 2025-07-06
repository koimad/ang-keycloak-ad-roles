import { Component } from '@angular/core';
import { MatCardHeader, MatCardActions, MatCardContent,MatCardTitle,MatCard} from '@angular/material/card';
import { MatButton } from '@angular/material/button';
import { MatIcon} from '@angular/material/icon';
import { AuthService } from '../../services/AuthService';

@Component({
  selector: 'app-forbidden',
  imports: [     
    MatButton,
    //MatIconModule,
    MatCardHeader,
    MatCardActions,
    MatCardContent,
    MatCardTitle,
    MatCard,
    MatIcon   
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
