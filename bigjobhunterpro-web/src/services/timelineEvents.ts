import api from './api';
import type {
  CreateTimelineEventRequest,
  UpdateTimelineEventRequest,
  TimelineEvent,
  TimelineEventsListResponse,
} from '../types/timelineEvent';

export const timelineEventsService = {
  create: async (
    applicationId: string,
    data: CreateTimelineEventRequest
  ): Promise<TimelineEvent> => {
    const response = await api.post<TimelineEvent>(
      `/api/applications/${applicationId}/timeline-events`,
      data
    );
    return response.data;
  },

  list: async (applicationId: string): Promise<TimelineEventsListResponse> => {
    const response = await api.get<TimelineEventsListResponse>(
      `/api/applications/${applicationId}/timeline-events`
    );
    return response.data;
  },

  update: async (
    applicationId: string,
    eventId: string,
    data: UpdateTimelineEventRequest
  ): Promise<TimelineEvent> => {
    const response = await api.put<TimelineEvent>(
      `/api/applications/${applicationId}/timeline-events/${eventId}`,
      data
    );
    return response.data;
  },

  delete: async (applicationId: string, eventId: string): Promise<void> => {
    await api.delete(`/api/applications/${applicationId}/timeline-events/${eventId}`);
  },
};
