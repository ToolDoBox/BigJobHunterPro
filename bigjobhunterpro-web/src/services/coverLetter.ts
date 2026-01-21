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

interface CombinedPdfResponse {
  blob: Blob;
  fileName: string;
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

const parseFilename = (contentDisposition?: string): string | null => {
  if (!contentDisposition) return null;

  const filenameMatch = /filename\*?=(?:UTF-8''|")?([^\";]+)/i.exec(contentDisposition);
  if (!filenameMatch?.[1]) return null;

  try {
    return decodeURIComponent(filenameMatch[1]);
  } catch {
    return filenameMatch[1];
  }
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

  async downloadCombinedPdf(applicationId: string): Promise<CombinedPdfResponse> {
    try {
      const response = await api.get<Blob>(
        `/api/applications/${applicationId}/cover-letter/combined-pdf`,
        { responseType: 'blob' }
      );
      const contentDisposition = response.headers['content-disposition'];
      const fileName = parseFilename(contentDisposition) ?? 'cover-letter-resume.pdf';
      return { blob: response.data, fileName };
    } catch (error) {
      let detailedMessage: string | null = null;
      if (axios.isAxiosError(error)) {
        const contentType = error.response?.headers?.['content-type'];
        if (contentType?.includes('application/json') && error.response?.data instanceof Blob) {
          try {
            const text = await error.response.data.text();
            const parsed = JSON.parse(text) as { error?: string };
            detailedMessage = parsed?.error ?? null;
          } catch {
            detailedMessage = null;
          }
        }
      }
      throw new Error(detailedMessage ?? parseError(error));
    }
  },
};

export default coverLetterService;
