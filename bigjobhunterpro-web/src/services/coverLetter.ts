import axios from 'axios';
import api from './api';

export interface CoverLetterResult {
  success: boolean;
  errorMessage: string | null;
  coverLetterHtml: string | null;
  generatedAt: string | null;
}

export interface GenerateCoverLetterRequest {
  resumeText?: string;
}

export interface SaveCoverLetterRequest {
  coverLetterHtml: string;
}

const parseError = (error: unknown): string => {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as { error?: string } | undefined;
    if (data?.error) {
      return data.error;
    }
    const status = error.response?.status;
    if (status === 401) {
      return 'Your session expired. Please log in again.';
    }
    if (status === 404) {
      return 'Application not found.';
    }
    if (status && status >= 500) {
      return 'Server error. Please try again.';
    }
  }
  return 'Something went wrong. Please try again.';
};

export const coverLetterService = {
  async generate(applicationId: string, resumeText?: string): Promise<CoverLetterResult> {
    try {
      const response = await api.post<CoverLetterResult>(
        `/api/applications/${applicationId}/cover-letter/generate`,
        { resumeText } as GenerateCoverLetterRequest
      );
      return response.data;
    } catch (error) {
      const message = parseError(error);
      return {
        success: false,
        errorMessage: message,
        coverLetterHtml: null,
        generatedAt: null,
      };
    }
  },

  async save(applicationId: string, coverLetterHtml: string): Promise<CoverLetterResult> {
    try {
      const response = await api.put<CoverLetterResult>(
        `/api/applications/${applicationId}/cover-letter`,
        { coverLetterHtml } as SaveCoverLetterRequest
      );
      return response.data;
    } catch (error) {
      const message = parseError(error);
      return {
        success: false,
        errorMessage: message,
        coverLetterHtml: null,
        generatedAt: null,
      };
    }
  },
};

export default coverLetterService;
