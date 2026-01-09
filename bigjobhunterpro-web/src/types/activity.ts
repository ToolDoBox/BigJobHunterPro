export interface ActivityEvent {
  id: string;
  partyId: string;
  userId: string;
  userDisplayName: string;
  eventType: string;
  pointsDelta: number;
  createdDate: string;
  companyName?: string | null;
  roleTitle?: string | null;
  milestoneLabel?: string | null;
}

export interface ActivityFeedResponse {
  partyId: string;
  events: ActivityEvent[];
  hasMore: boolean;
}
