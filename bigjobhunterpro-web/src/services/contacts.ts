import axios from 'axios';
import api from './api';
import type {
  Contact,
  CreateContactRequest,
  UpdateContactRequest,
  ContactsListResponse,
} from '../types/contact';

const parseContactError = (error: unknown, action: string): string => {
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
      return `You do not have permission to ${action} for this application.`;
    }
    if (status === 404) {
      return `We could not find that contact to ${action}. It may have been removed.`;
    }
    if (status >= 500) {
      return `Server error while ${action}. Please try again in a moment.`;
    }

    return `Unable to ${action}. Please try again.`;
  }

  return `Unexpected error while ${action}. Please try again.`;
};

export const contactsService = {
  create: async (
    applicationId: string,
    data: CreateContactRequest
  ): Promise<Contact> => {
    try {
      const response = await api.post<Contact>(
        `/api/applications/${applicationId}/contacts`,
        data
      );
      return response.data;
    } catch (error) {
      throw new Error(parseContactError(error, 'adding this contact'));
    }
  },

  list: async (applicationId: string): Promise<ContactsListResponse> => {
    try {
      const response = await api.get<ContactsListResponse>(
        `/api/applications/${applicationId}/contacts`
      );
      return response.data;
    } catch (error) {
      throw new Error(parseContactError(error, 'loading contacts'));
    }
  },

  update: async (
    applicationId: string,
    contactId: string,
    data: UpdateContactRequest
  ): Promise<Contact> => {
    try {
      const response = await api.put<Contact>(
        `/api/applications/${applicationId}/contacts/${contactId}`,
        data
      );
      return response.data;
    } catch (error) {
      throw new Error(parseContactError(error, 'updating this contact'));
    }
  },

  delete: async (applicationId: string, contactId: string): Promise<void> => {
    try {
      await api.delete(`/api/applications/${applicationId}/contacts/${contactId}`);
    } catch (error) {
      throw new Error(parseContactError(error, 'deleting this contact'));
    }
  },
};
