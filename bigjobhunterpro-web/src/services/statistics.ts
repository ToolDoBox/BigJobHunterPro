import api from './api';

// Type definitions
export interface WeeklyStats {
  applicationsThisWeek: number;
  applicationsLastWeek: number;
  percentageChange: number;
  pointsThisWeek: number;
  pointsLastWeek: number;
}

export interface StatusDistribution {
  status: string;
  count: number;
  percentage: number;
}

export interface StatusDistributionResponse {
  statuses: StatusDistribution[];
  totalApplications: number;
}

export interface SourceDistribution {
  sourceName: string;
  count: number;
  percentage: number;
}

export interface SourceDistributionResponse {
  sources: SourceDistribution[];
  totalApplications: number;
}

export interface AverageTimeToMilestone {
  milestone: string;
  averageDays: number | null;
  sampleSize: number;
}

export interface AverageTimeResponse {
  milestones: AverageTimeToMilestone[];
}

/**
 * Get weekly statistics comparing this week vs last week
 * Results are cached for 5 minutes on the server
 */
export const getWeeklyStats = async (): Promise<WeeklyStats> => {
  const response = await api.get<WeeklyStats>('/api/statistics/weekly');
  return response.data;
};

/**
 * Get distribution of applications across different statuses
 * Results are cached for 5 minutes on the server
 */
export const getStatusDistribution = async (): Promise<StatusDistributionResponse> => {
  const response = await api.get<StatusDistributionResponse>('/api/statistics/status-distribution');
  return response.data;
};

/**
 * Get distribution of applications across different sources
 * Results are cached for 5 minutes on the server
 */
export const getSourceDistribution = async (): Promise<SourceDistributionResponse> => {
  const response = await api.get<SourceDistributionResponse>('/api/statistics/source-distribution');
  return response.data;
};

/**
 * Get average time from application submission to various milestones
 * Results are cached for 5 minutes on the server
 */
export const getAverageTime = async (): Promise<AverageTimeResponse> => {
  const response = await api.get<AverageTimeResponse>('/api/statistics/average-time');
  return response.data;
};
