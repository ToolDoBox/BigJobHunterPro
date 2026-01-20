import type { TimelineEvent } from './timelineEvent';

export interface CreateApplicationRequest {
  sourceUrl?: string;
  rawPageContent: string;
}

export interface CreateApplicationResponse {
  id: string;
  sourceUrl: string | null;
  status: string;
  aiParsingStatus: string;
  points: number;
  totalPoints: number;
  createdDate: string;
}

export interface ApplicationListItem {
  id: string;
  companyName: string;
  roleTitle: string;
  status: string;
  createdDate: string;
}

export interface ApplicationsListResponse {
  items: ApplicationListItem[];
  page: number;
  pageSize: number;
  hasMore: boolean;
}

export interface ApplicationDetail {
  id: string;
  companyName: string;
  roleTitle: string;
  sourceName: string;
  sourceUrl: string | null;
  status: string;
  workMode: string | null;
  location: string | null;
  salaryMin: number | null;
  salaryMax: number | null;
  jobDescription: string | null;
  requiredSkills: string[];
  niceToHaveSkills: string[];
  parsedByAI: boolean;
  aiParsingStatus: string;
  points: number;
  createdDate: string;
  updatedDate: string;
  lastAIParsedDate: string | null;
  rawPageContent: string | null;
  coverLetterHtml: string | null;
  coverLetterGeneratedAt: string | null;
  timelineEvents: TimelineEvent[];
}

export interface UpdateApplicationRequest {
  companyName: string;
  roleTitle: string;
  sourceName: string;
  sourceUrl?: string;
  status: string;
  workMode?: string;
  location?: string;
  salaryMin?: number | null;
  salaryMax?: number | null;
  jobDescription?: string;
  requiredSkills: string[];
  niceToHaveSkills: string[];
  rawPageContent?: string;
}

export interface ApplicationError {
  type?: string;
  title?: string;
  status?: number;
  error?: string;
  details?: string[];
  errors?: Record<string, string[]>;
}
