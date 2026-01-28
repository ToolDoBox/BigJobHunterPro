import axios from 'axios';
import api from './api';
import type {
  InterviewQuestion,
  CreateInterviewQuestionRequest,
  UpdateInterviewQuestionRequest,
  InterviewQuestionsListResponse,
  QuestionCategory,
} from '../types/question';

const parseQuestionError = (error: unknown, action: string): string => {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as { error?: string; message?: string } | undefined;

    if (!error.response) {
      if (error.code === 'ECONNABORTED' || error.message.toLowerCase().includes('timeout')) {
        return `Timed out while ${action}. Please try again.`;
      }
      return `Unable to reach the server while ${action}. Check your connection and try again.`;
    }

    if (data?.error) {
      return data.error;
    }
    if (data?.message) {
      return data.message;
    }

    const status = error.response.status;
    if (status === 400) {
      return `We could not ${action} because some details were invalid. Please review the form and try again.`;
    }
    if (status === 401) {
      return `You were signed out while ${action}. Please log in and try again.`;
    }
    if (status === 403) {
      return `You do not have permission to ${action}.`;
    }
    if (status === 404) {
      return `We could not find that question to ${action}. It may have been removed.`;
    }
    if (status >= 500) {
      return `Server error while ${action}. Please try again in a moment.`;
    }

    return `Unable to ${action}. Please try again.`;
  }

  return `Unexpected error while ${action}. Please try again.`;
};

export interface GetQuestionsParams {
  category?: QuestionCategory;
  search?: string;
  applicationId?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDescending?: boolean;
}

export const questionsService = {
  getAll: async (params: GetQuestionsParams = {}): Promise<InterviewQuestionsListResponse> => {
    try {
      const response = await api.get<InterviewQuestionsListResponse>('/api/interview-questions', {
        params: {
          category: params.category,
          search: params.search,
          applicationId: params.applicationId,
          page: params.page || 1,
          pageSize: params.pageSize || 20,
          sortBy: params.sortBy || 'createdDate',
          sortDescending: params.sortDescending ?? true,
        },
      });
      return response.data;
    } catch (error) {
      throw new Error(parseQuestionError(error, 'loading questions'));
    }
  },

  getById: async (id: string): Promise<InterviewQuestion> => {
    try {
      const response = await api.get<InterviewQuestion>(`/api/interview-questions/${id}`);
      return response.data;
    } catch (error) {
      throw new Error(parseQuestionError(error, 'loading this question'));
    }
  },

  create: async (data: CreateInterviewQuestionRequest): Promise<InterviewQuestion> => {
    try {
      const response = await api.post<InterviewQuestion>('/api/interview-questions', data);
      return response.data;
    } catch (error) {
      throw new Error(parseQuestionError(error, 'creating this question'));
    }
  },

  update: async (id: string, data: UpdateInterviewQuestionRequest): Promise<InterviewQuestion> => {
    try {
      const response = await api.put<InterviewQuestion>(`/api/interview-questions/${id}`, data);
      return response.data;
    } catch (error) {
      throw new Error(parseQuestionError(error, 'updating this question'));
    }
  },

  delete: async (id: string): Promise<void> => {
    try {
      await api.delete(`/api/interview-questions/${id}`);
    } catch (error) {
      throw new Error(parseQuestionError(error, 'deleting this question'));
    }
  },

  incrementTimesAsked: async (id: string): Promise<InterviewQuestion> => {
    try {
      const response = await api.post<InterviewQuestion>(`/api/interview-questions/${id}/increment`);
      return response.data;
    } catch (error) {
      throw new Error(parseQuestionError(error, 'updating question frequency'));
    }
  },

  getFrequent: async (limit: number = 10): Promise<InterviewQuestion[]> => {
    try {
      const response = await api.get<InterviewQuestion[]>('/api/interview-questions/frequent', {
        params: { limit },
      });
      return response.data;
    } catch (error) {
      throw new Error(parseQuestionError(error, 'loading frequent questions'));
    }
  },
};
