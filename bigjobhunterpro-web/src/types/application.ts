export interface CreateApplicationRequest {
  companyName: string;
  roleTitle: string;
  sourceName: string;
  sourceUrl?: string;
}

export interface CreateApplicationResponse {
  id: string;
  companyName: string;
  roleTitle: string;
  sourceName: string;
  sourceUrl: string | null;
  status: string;
  points: number;
  totalPoints: number;
  createdDate: string;
}

export interface ApplicationError {
  type?: string;
  title?: string;
  status?: number;
  errors?: Record<string, string[]>;
}
