import axios from 'axios';
import api from './api';
import type {
  HuntingParty,
  HuntingPartyDetail,
  CreateHuntingPartyRequest,
  JoinHuntingPartyRequest,
  LeaderboardEntry,
  RivalryData,
} from '@/types/huntingParty';
import type { ActivityFeedResponse } from '@/types/activity';

const parseError = (error: unknown): string => {
  if (axios.isAxiosError(error)) {
    // Handle network errors (no response from server)
    if (!error.response) {
      if (error.code === 'ECONNABORTED') {
        return 'Request timed out. Please check your connection and try again.';
      }
      return 'Unable to connect to server. Please check your internet connection.';
    }

    const status = error.response.status;
    const data = error.response.data as { error?: string; message?: string } | undefined;

    // Try to get error message from response
    if (data?.error) {
      return data.error;
    }
    if (data?.message) {
      return data.message;
    }

    // Provide specific messages for common HTTP status codes
    switch (status) {
      case 400:
        return 'Invalid request. Please check your input.';
      case 401:
        return 'Please log in to continue.';
      case 403:
        return 'You do not have permission to perform this action.';
      case 404:
        return 'Not found.';
      case 409:
        return 'A conflict occurred. Please refresh and try again.';
      case 500:
        return 'Server error. Please try again later.';
      default:
        return `Request failed (${status}). Please try again.`;
    }
  }

  // Handle non-Axios errors
  if (error instanceof Error) {
    return error.message;
  }

  return 'An unexpected error occurred. Please try again.';
};

export const huntingPartiesService = {
  async createParty(data: CreateHuntingPartyRequest): Promise<HuntingParty> {
    try {
      const response = await api.post<HuntingParty>('/api/parties', data);
      return response.data;
    } catch (error) {
      throw new Error(parseError(error));
    }
  },

  async getUserParty(): Promise<HuntingParty | null> {
    try {
      const response = await api.get<HuntingParty>('/api/parties');
      return response.data;
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status === 204) {
        return null;
      }
      throw new Error(parseError(error));
    }
  },

  async getPartyDetail(partyId: string): Promise<HuntingPartyDetail> {
    try {
      const response = await api.get<HuntingPartyDetail>(`/api/parties/${partyId}`);
      return response.data;
    } catch (error) {
      throw new Error(parseError(error));
    }
  },

  async joinParty(data: JoinHuntingPartyRequest): Promise<HuntingParty> {
    try {
      const response = await api.post<HuntingParty>('/api/parties/join', data);
      return response.data;
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status === 404) {
        throw new Error('Invalid invite code');
      }
      throw new Error(parseError(error));
    }
  },

  async leaveParty(partyId: string): Promise<void> {
    try {
      await api.delete(`/api/parties/${partyId}/leave`);
    } catch (error) {
      throw new Error(parseError(error));
    }
  },

  async getLeaderboard(partyId: string): Promise<LeaderboardEntry[]> {
    try {
      const response = await api.get<LeaderboardEntry[]>(`/api/parties/${partyId}/leaderboard`);
      return response.data;
    } catch (error) {
      throw new Error(parseError(error));
    }
  },

  async getRivalry(partyId: string): Promise<RivalryData> {
    try {
      const response = await api.get<RivalryData>(`/api/parties/${partyId}/rivalry`);
      return response.data;
    } catch (error) {
      throw new Error(parseError(error));
    }
  },

  async getActivityFeed(partyId: string, limit = 50): Promise<ActivityFeedResponse> {
    try {
      const response = await api.get<ActivityFeedResponse>(`/api/parties/${partyId}/activity`, {
        params: { limit },
      });
      return response.data;
    } catch (error) {
      throw new Error(parseError(error));
    }
  },
};

export default huntingPartiesService;
