// core/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';

import { AuthResponse, LoginRequest } from '../../shared/models/auth.models';
import { TokenService } from './token.service';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/auth`;

  private isAuthenticatedSubject: BehaviorSubject<boolean>;
  isAuthenticated$: Observable<boolean>;

  constructor(
    private http: HttpClient,
    private router: Router,
    private tokenService: TokenService
  ) {
    this.isAuthenticatedSubject = new BehaviorSubject<boolean>(this.tokenService.isValid());
    this.isAuthenticated$ = this.isAuthenticatedSubject.asObservable();
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, request).pipe(
      tap(response => this.storeToken(response.token))
    );
  }

  logout(): void {
    this.tokenService.clear();
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['/auth/login']);
  }

  isLoggedIn(): boolean {
    return this.tokenService.isValid();
  }

  getToken(): string | null {
    return this.tokenService.get();
  }

  getRoles(): string[] {
    return this.tokenService.getUser()?.roles ?? [];
  }

  getEmail(): string {
    return this.tokenService.getUser()?.email ?? '';
  }

  private storeToken(token: string): void {
    this.tokenService.set(token);
    this.isAuthenticatedSubject.next(true);
  }
}
