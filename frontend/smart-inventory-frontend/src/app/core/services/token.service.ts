import { Injectable } from '@angular/core';

export interface TokenUser {
  email: string;
  roles: string[];
  exp?: number;
}

@Injectable({ providedIn: 'root' })
export class TokenService {
  private readonly KEY = 'auth_token';

  get(): string | null {
    try {
      return localStorage.getItem(this.KEY);
    } catch {
      return null;
    }
  }

  set(token: string): void {
    try {
      localStorage.setItem(this.KEY, token);
    } catch {
      // Silently ignore storage failures in browsers with disabled storage
    }
  }

  clear(): void {
    try {
      localStorage.removeItem(this.KEY);
    } catch {
      // ignore
    }
  }

  getUser(): TokenUser | null {
    const token = this.get();
    const payload = this.decodePayload(token);
    if (!payload) return null;

    const roleClaim = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? payload.role;
    const roles = Array.isArray(roleClaim)
      ? roleClaim
      : roleClaim
        ? [roleClaim]
        : [];

    return {
      email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ?? payload.email ?? '',
      roles,
      exp: payload.exp
    };
  }

  isValid(): boolean {
    const token = this.get();
    const payload = this.decodePayload(token);
    if (!payload || typeof payload.exp !== 'number') {
      return false;
    }

    return payload.exp * 1000 > Date.now();
  }

  private decodePayload(token: string | null): any | null {
    if (!token) return null;

    try {
      const parts = token.split('.');
      if (parts.length !== 3) return null;

      const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
      const padded = base64.padEnd(base64.length + (4 - (base64.length % 4)) % 4, '=');
      const decoded = atob(padded);
      const json = decodeURIComponent(
        decoded
          .split('')
          .map(c => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
          .join('')
      );

      return JSON.parse(json);
    } catch {
      return null;
    }
  }
}
