import api from './api';

// Type definitions
export interface KeywordFrequency {
  keyword: string;
  frequency: number;
  percentage: number;
}

export interface ConversionBySource {
  sourceName: string;
  totalApplications: number;
  interviewCount: number;
  conversionRate: number;
}

// Explicit type exports for better compatibility
export type { KeywordFrequency, ConversionBySource };

/**
 * Get top keywords from successful applications (those that reached interview stage or beyond)
 * Results are cached for 10 minutes on the server
 * @param topCount Number of top keywords to return (default: 20, max: 50)
 */
export const getTopKeywords = async (topCount: number = 20): Promise<KeywordFrequency[]> => {
  const response = await api.get<KeywordFrequency[]>('/api/analytics/keywords', {
    params: { topCount }
  });
  return response.data;
};

/**
 * Get conversion rates by application source/platform
 * Shows which job boards or sources have the highest success rates
 * Results are cached for 10 minutes on the server
 */
export const getConversionBySource = async (): Promise<ConversionBySource[]> => {
  const response = await api.get<ConversionBySource[]>('/api/analytics/conversion-by-source');
  return response.data;
};
