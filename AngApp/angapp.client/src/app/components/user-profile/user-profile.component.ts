import { Component, OnInit } from '@angular/core';
import { User } from '../../models/user.model';
import { AuthService } from '../../services/AuthService';

@Component({
  selector: 'app-user-profile',
  templateUrl: 'user-profile.component.html',
  styleUrls: [`user-profile.component.scss`]
})
export class UserProfileComponent implements OnInit {
  
  user: User | null = null;

  roles: string[] = [];

  constructor(private readonly authService: AuthService) { }

  async ngOnInit() {
     if (this.authService?.isLoggedIn) {
         this.user = this.authService.user;
     }
  }
}
