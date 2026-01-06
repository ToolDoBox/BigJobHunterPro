export type EventType =
  | 'Prospecting'
  | 'Applied'
  | 'Screening'
  | 'Interview'
  | 'Offer'
  | 'Rejected'
  | 'Withdrawn';

export interface TimelineEvent {
  id: string;
  applicationId: string;
  eventType: EventType;
  interviewRound: number | null;
  timestamp: string;
  notes: string | null;
  points: number;
  createdDate: string;
}

export interface CreateTimelineEventRequest {
  eventType: EventType;
  interviewRound?: number;
  timestamp: string;
  notes?: string;
}

export interface UpdateTimelineEventRequest {
  eventType: EventType;
  interviewRound?: number;
  timestamp: string;
  notes?: string;
}

export interface TimelineEventsListResponse {
  applicationId: string;
  events: TimelineEvent[];
  totalPoints: number;
  currentStatus: string;
}
