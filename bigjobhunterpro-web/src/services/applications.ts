import axios from 'axios';
import api from './api';
import type {
  CreateApplicationRequest,
  CreateApplicationResponse,
  ApplicationError
} from '@/types/application';

const parseError = (error: unknown): Record<string, string[]> => {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as ApplicationError | undefined;

    if (data?.errors) {
      return data.errors;
    }

    return { general: ['An unexpected error occurred. Please try again.'] };
  }

  return { general: ['Network error. Please check your connection.'] };
};

export const applicationsService = {
  async createApplication(data: CreateApplicationRequest): Promise<CreateApplicationResponse> {
    try {
      const response = await api.post<CreateApplicationResponse>('/api/applications', data);
      return response.data;
    } catch (error) {
      const errors = parseError(error);
      throw { errors };
    }
  },
};

export default applicationsService;
