export type ContactRelationship = 'Recruiter' | 'HiringManager' | 'TeamMember' | 'Other';

export interface Contact {
  id: string;
  name: string;
  role: string | null;
  relationship: ContactRelationship;
  emails: string[];
  phones: string[];
  linkedin: string | null;
  notes: string | null;
  createdDate: string;
  updatedDate: string | null;
}

export interface CreateContactRequest {
  name: string;
  role?: string;
  relationship: ContactRelationship;
  emails: string[];
  phones: string[];
  linkedin?: string;
  notes?: string;
}

export interface UpdateContactRequest {
  name: string;
  role?: string;
  relationship: ContactRelationship;
  emails: string[];
  phones: string[];
  linkedin?: string;
  notes?: string;
}

export interface ContactsListResponse {
  applicationId: string;
  contacts: Contact[];
  totalCount: number;
}
