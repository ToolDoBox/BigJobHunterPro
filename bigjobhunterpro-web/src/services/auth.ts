import axios from 'axios';
import api, { tokenStorage } from './api';

// Type definitions
export interface User {
  userId: string;
  email: string;
  displayName: string;
  points: number;
  totalPoints: number;
  applicationCount: number;
  currentStreak: number;
  longestStreak: number;
  lastActivityDate?: string; // ISO 8601 UTC
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

// Parse error response from API with detailed, user-friendly messages
const parseError = (error: unknown, context: 'login' | 'register' = 'login'): string => {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as AuthError | undefined;

    // First, try to use the server's error message if it's detailed enough
    if (data?.error) {
      // Check if it's a generic error that we can improve
      if (data.error.toLowerCase().includes('invalid credentials') ||
          data.error.toLowerCase().includes('invalid email or password')) {
        return '❌ Login failed: The email or password you entered is incorrect. Please check your credentials and try again.';
      }

      // If server provides a good error message, use it
      if (data.error.length > 20) {
        return data.error;
      }
    }

    // Use details array if available
    if (data?.details && data.details.length > 0) {
      const details = data.details.join('. ');
      if (details.length > 20) {
        return details;
      }
    }

    // Handle common HTTP status codes with context-aware messages
    const status = error.response?.status;

    switch (status) {
      case 400:
        if (context === 'login') {
          return '❌ Invalid login request. Please ensure your email is in the correct format and your password is at least 8 characters.';
        }
        return '❌ Invalid request. Please check all required fields are filled correctly.';

      case 401:
        if (error.response?.data?.error?.toLowerCase().includes('locked')) {
          return '🔒 Your account has been temporarily locked due to multiple failed login attempts. Please try again in 5 minutes.';
        }
        if (context === 'login') {
          return '❌ Login failed: The email or password you entered is incorrect. Please double-check your credentials.\n\n💡 Tip: Use the 👁️ icon to verify your password is typed correctly.';
        }
        return '❌ Authentication failed. Please check your credentials.';

      case 403:
        return '🚫 Access denied. Your account may be disabled or pending verification. Please contact support if this continues.';

      case 404:
        if (context === 'login') {
          return '❌ No account found with this email address. Please check the email or create a new account.';
        }
        return '❌ Resource not found. Please try again.';

      case 409:
        return '⚠️ This email address is already registered. Try logging in instead, or use a different email address.';

      case 429:
        return '⏱️ Too many attempts. Please wait a few minutes before trying again to prevent account lockout.';

      case 500:
        return '🔧 Server error: Our system encountered an issue. Please try again in a few moments. If this persists, contact support.';

      case 502:
      case 503:
      case 504:
        return '🌐 Service temporarily unavailable. Our servers may be updating. Please try again in a minute.';

      default:
        // If we have any response at all, it's a server error
        if (error.response) {
          return `⚠️ Unexpected server response (${status}). Please try again or contact support if this continues.`;
        }

        // Network/connection errors
        if (error.code === 'ECONNABORTED' || error.message.includes('timeout')) {
          return '⏱️ Connection timeout: The server took too long to respond. Please check your internet connection and try again.';
        }

        if (error.code === 'ERR_NETWORK' || error.message.includes('Network Error')) {
          return '🌐 Network error: Unable to reach the server. Please check your internet connection and try again.';
        }

        if (error.message.includes('CORS')) {
          return '🔧 Connection error: Unable to communicate with the server. This may be a temporary issue - please try again.';
        }

        return '❌ An unexpected error occurred. Please check your internet connection and try again.';
    }
  }

  // Non-Axios errors (shouldn't happen, but just in case)
  if (error instanceof Error) {
    return `⚠️ Error: ${error.message}. Please try again.`;
  }

  return '❌ An unexpected error occurred. Please check your internet connection and try again.';
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
      throw new Error(parseError(error, 'login'));
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
      throw new Error(parseError(error, 'register'));
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
