import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent {
  userEmail = '';

  constructor(private authService: AuthService, private router: Router) {
    this.userEmail = this.authService.getEmail();
  }

  logout(): void {
    this.authService.logout();
  }
}
