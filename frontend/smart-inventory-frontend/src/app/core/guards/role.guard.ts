
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({ providedIn: 'root' })
export class RoleGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const requiredRole = route.data['role'] as string;
    const userRoles = this.authService.getRoles();

    if (requiredRole && userRoles.includes(requiredRole)) {
      return true;
    }

    this.router.navigate(['/dashboard']);
    return false;
  }
}
