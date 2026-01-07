import axios from 'axios';
import api from './api';
import type {
  CreateTimelineEventRequest,
  UpdateTimelineEventRequest,
  TimelineEvent,
  TimelineEventsListResponse,
} from '../types/timelineEvent';

const parseTimelineError = (error: unknown, action: string): string => {
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
      return `We could not find that timeline event to ${action}. It may have been removed.`;
    }
    if (status >= 500) {
      return `Server error while ${action}. Please try again in a moment.`;
    }

    return `Unable to ${action}. Please try again.`;
  }

  return `Unexpected error while ${action}. Please try again.`;
};

export const timelineEventsService = {
  create: async (
    applicationId: string,
    data: CreateTimelineEventRequest
  ): Promise<TimelineEvent> => {
    try {
      const response = await api.post<TimelineEvent>(
        `/api/applications/${applicationId}/timeline-events`,
        data
      );
      return response.data;
    } catch (error) {
      throw new Error(parseTimelineError(error, 'adding this timeline event'));
    }
  },

  list: async (applicationId: string): Promise<TimelineEventsListResponse> => {
    try {
      const response = await api.get<TimelineEventsListResponse>(
        `/api/applications/${applicationId}/timeline-events`
      );
      return response.data;
    } catch (error) {
      throw new Error(parseTimelineError(error, 'loading timeline events'));
    }
  },

  update: async (
    applicationId: string,
    eventId: string,
    data: UpdateTimelineEventRequest
  ): Promise<TimelineEvent> => {
    try {
      const response = await api.put<TimelineEvent>(
        `/api/applications/${applicationId}/timeline-events/${eventId}`,
        data
      );
      return response.data;
    } catch (error) {
      throw new Error(parseTimelineError(error, 'updating this timeline event'));
    }
  },

  delete: async (applicationId: string, eventId: string): Promise<void> => {
    try {
      await api.delete(`/api/applications/${applicationId}/timeline-events/${eventId}`);
    } catch (error) {
      throw new Error(parseTimelineError(error, 'deleting this timeline event'));
    }
  },
};
