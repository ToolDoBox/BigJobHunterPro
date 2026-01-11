import api from './api';

// Type definitions
export interface WeeklyStats {
  applicationsThisWeek: number;
  applicationsLastWeek: number;
  percentageChange: number;
  pointsThisWeek: number;
  pointsLastWeek: number;
}

/**
 * Get weekly statistics comparing this week vs last week
 * Results are cached for 5 minutes on the server
 */
export const getWeeklyStats = async (): Promise<WeeklyStats> => {
  const response = await api.get<WeeklyStats>('/api/statistics/weekly');
  return response.data;
};
