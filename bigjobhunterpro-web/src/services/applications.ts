import axios from 'axios';
import api from './api';
import type {
  CreateApplicationRequest,
  CreateApplicationResponse,
  ApplicationsListResponse,
  ApplicationDetail,
  UpdateApplicationRequest,
  ApplicationError
} from '@/types/application';

const mapDetailsToErrors = (details: string[]): Record<string, string[]> => {
  const errors: Record<string, string[]> = {};
  const general: string[] = [];

  details.forEach((message) => {
    const normalized = message.toLowerCase();

    if (normalized.includes('company')) {
      errors.companyName = [message];
    } else if (normalized.includes('role')) {
      errors.roleTitle = [message];
    } else if (normalized.includes('status')) {
      errors.status = [message];
    } else if (normalized.includes('work mode') || normalized.includes('workmode')) {
      errors.workMode = [message];
    } else if (normalized.includes('location')) {
      errors.location = [message];
    } else if (normalized.includes('salary minimum')) {
      errors.salaryMin = [message];
    } else if (normalized.includes('salary maximum')) {
      errors.salaryMax = [message];
    } else if (normalized.includes('salary')) {
      errors.salaryMin = [message];
    } else if (normalized.includes('source url') || normalized.includes('url')) {
      errors.sourceUrl = [message];
    } else if (normalized.includes('source')) {
      errors.sourceName = [message];
    } else if (normalized.includes('job description')) {
      errors.jobDescription = [message];
    } else if (normalized.includes('required skills')) {
      errors.requiredSkills = [message];
    } else if (normalized.includes('nice') || normalized.includes('have skills')) {
      errors.niceToHaveSkills = [message];
    } else if (normalized.includes('job page') || normalized.includes('raw page')) {
      errors.rawPageContent = [message];
    } else {
      general.push(message);
    }
  });

  if (general.length > 0) {
    errors.general = general;
  }

  return errors;
};

const parseError = (error: unknown): Record<string, string[]> => {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as ApplicationError | undefined;

    if (data?.errors) {
      return data.errors;
    }

    if (data?.details && data.details.length > 0) {
      return mapDetailsToErrors(data.details);
    }

    if (data?.error) {
      return { general: [data.error] };
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
  async getApplications(page = 1, pageSize = 25): Promise<ApplicationsListResponse> {
    try {
      const response = await api.get<ApplicationsListResponse>('/api/applications', {
        params: { page, pageSize },
      });
      return response.data;
    } catch (error) {
      const errors = parseError(error);
      const message = errors.general?.[0] ?? 'Unable to load applications.';
      throw new Error(message);
    }
  },
  async getApplication(id: string): Promise<ApplicationDetail> {
    try {
      const response = await api.get<ApplicationDetail>(`/api/applications/${id}`);
      return response.data;
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status === 404) {
        const notFoundError = new Error('NOT_FOUND');
        (notFoundError as Error & { code?: string }).code = 'NOT_FOUND';
        throw notFoundError;
      }
      const errors = parseError(error);
      const message = errors.general?.[0] ?? 'Unable to load application.';
      throw new Error(message);
    }
  },
  async updateApplication(id: string, data: UpdateApplicationRequest): Promise<ApplicationDetail> {
    try {
      const response = await api.put<ApplicationDetail>(`/api/applications/${id}`, data);
      return response.data;
    } catch (error) {
      const errors = parseError(error);
      throw { errors };
    }
  },
  async deleteApplication(id: string): Promise<void> {
    try {
      await api.delete(`/api/applications/${id}`);
    } catch (error) {
      const errors = parseError(error);
      const message = errors.general?.[0] ?? 'Unable to delete application.';
      throw new Error(message);
    }
  },
};

export default applicationsService;
