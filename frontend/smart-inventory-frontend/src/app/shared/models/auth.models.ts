export interface LoginRequest {
  email: string;
  password: string;
}
export interface RegisterRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  email: string;
  roles: string[];
  expiresAt: string;
}

export interface DecodedToken {
  sub: string;
  email: string;
  role: string | string[];
  exp: number;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string | string[];
}
