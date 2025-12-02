// Login
export interface UserLogin {
  email: string;
  password: string;
  tenant: string; // from header or subdomain
}

// Token Response
export interface TokenData {
  token: string;
  refreshToken: string;
  refreshTokenExpiryTime: string;
}

// Forgot Password
export interface ForgotPasswordRequest {
  email: string;
  tenant: string;
}

// Reset Password
export interface ResetPasswordRequest {
  email: string;
  token: string;
  password: string;
  confirmPassword: string;
  tenant: string;
}
