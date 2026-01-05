import axios from 'axios';
import api, { tokenStorage } from './api';

// Type definitions
export interface User {
  userId: string;
  email: string;
  displayName: string;
  points: number;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  userId: string;
  email: string;
  token: string;
  expiresAt: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  displayName: string;
}

export interface RegisterResponse {
  userId: string;
  email: string;
  token: string;
}

export interface AuthError {
  error: string;
  details?: string[];
}

// Parse error response from API
const parseError = (error: unknown): string => {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as AuthError | undefined;

    if (data?.error) {
      return data.error;
    }

    if (data?.details && data.details.length > 0) {
      return data.details.join('. ');
    }

    // Handle common HTTP status codes
    switch (error.response?.status) {
      case 400:
        return 'Invalid request. Please check your input.';
      case 401:
        return 'Invalid email or password.';
      case 409:
        return 'Email already registered.';
      case 500:
        return 'Server error. Please try again later.';
      default:
        return 'An unexpected error occurred.';
    }
  }

  return 'Network error. Please check your connection.';
};

// Auth service functions
export const authService = {
  /**
   * Log in with email and password
   */
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    try {
      const response = await api.post<LoginResponse>('/api/auth/login', credentials);
      const data = response.data;

      // Store token
      tokenStorage.setToken(data.token);

      return data;
    } catch (error) {
      throw new Error(parseError(error));
    }
  },

  /**
   * Register a new user
   */
  async register(data: RegisterRequest): Promise<RegisterResponse> {
    try {
      const response = await api.post<RegisterResponse>('/api/auth/register', data);
      const result = response.data;

      // Store token (auto-login after registration)
      tokenStorage.setToken(result.token);

      return result;
    } catch (error) {
      throw new Error(parseError(error));
    }
  },

  /**
   * Log out (clear token and optionally notify server)
   */
  async logout(): Promise<void> {
    try {
      // Attempt to notify server (optional - don't block on failure)
      if (tokenStorage.hasToken()) {
        await api.post('/api/auth/logout').catch(() => {
          // Silently fail - we still want to clear local token
        });
      }
    } finally {
      tokenStorage.removeToken();
    }
  },

  /**
   * Get current user info
   */
  async getMe(): Promise<User> {
    try {
      const response = await api.get<User>('/api/auth/me');
      return response.data;
    } catch (error) {
      throw new Error(parseError(error));
    }
  },

  /**
   * Check if user has a valid token stored
   */
  isAuthenticated(): boolean {
    return tokenStorage.hasToken();
  },
};

export default authService;
