import axios from 'axios';
import api from './api';

// Type definitions
export interface ProfileResponse {
  displayName: string;
  email: string;
  resumeText: string | null;
  resumeUpdatedAt: string | null; // ISO 8601 UTC
  characterCount: number;
}

export interface UpdateResumeRequest {
  resumeText: string | null;
}

export interface UpdateResumeResponse {
  success: boolean;
  resumeUpdatedAt: string | null; // ISO 8601 UTC
  characterCount: number;
  message: string;
}

export interface ProfileError {
  error: string;
  details?: string[];
}

// Parse error response from API
const parseError = (error: unknown): string => {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as ProfileError | undefined;

    if (data?.error) {
      if (data.details && data.details.length > 0) {
        return `${data.error}: ${data.details.join('. ')}`;
      }
      return data.error;
    }

    const status = error.response?.status;
    switch (status) {
      case 400:
        return 'Invalid request. Please check your input.';
      case 401:
        return 'Please log in to continue.';
      case 404:
        return 'Profile not found.';
      case 500:
        return 'Server error. Please try again later.';
      default:
        return 'An unexpected error occurred. Please try again.';
    }
  }

  if (error instanceof Error) {
    return error.message;
  }

  return 'An unexpected error occurred. Please try again.';
};

// Profile service functions
export const profileService = {
  /**
   * Get current user's profile including resume
   */
  async getProfile(): Promise<ProfileResponse> {
    try {
      const response = await api.get<ProfileResponse>('/api/profile');
      return response.data;
    } catch (error) {
      throw new Error(parseError(error));
    }
  },

  /**
   * Update user's resume text
   */
  async updateResume(resumeText: string | null): Promise<UpdateResumeResponse> {
    try {
      const request: UpdateResumeRequest = { resumeText };
      const response = await api.put<UpdateResumeResponse>('/api/profile/resume', request);
      return response.data;
    } catch (error) {
      throw new Error(parseError(error));
    }
  },

  /**
   * Clear user's resume text
   */
  async clearResume(): Promise<UpdateResumeResponse> {
    try {
      const response = await api.delete<UpdateResumeResponse>('/api/profile/resume');
      return response.data;
    } catch (error) {
      throw new Error(parseError(error));
    }
  },
};

export default profileService;
