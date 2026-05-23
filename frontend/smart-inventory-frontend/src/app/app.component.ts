import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  isAuthenticated$: Observable<boolean>;

  constructor(private authService: AuthService) {
    this.isAuthenticated$ = this.authService.isAuthenticated$;
  }
}
