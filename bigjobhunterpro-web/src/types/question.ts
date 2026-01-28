export type QuestionCategory =
  | 'Behavioral'
  | 'Technical'
  | 'Situational'
  | 'CompanySpecific'
  | 'General'
  | 'Other';

export interface InterviewQuestion {
  id: string;
  questionText: string;
  answerText: string | null;
  notes: string | null;
  category: QuestionCategory;
  tags: string[];
  timesAsked: number;
  lastAskedDate: string | null;
  createdDate: string;
  updatedDate: string | null;
  applicationId: string | null;
  applicationCompany: string | null;
  applicationRole: string | null;
}

export interface CreateInterviewQuestionRequest {
  questionText: string;
  answerText?: string;
  notes?: string;
  category: QuestionCategory;
  tags: string[];
  applicationId?: string;
}

export interface UpdateInterviewQuestionRequest {
  questionText: string;
  answerText?: string;
  notes?: string;
  category: QuestionCategory;
  tags: string[];
  applicationId?: string;
}

export interface InterviewQuestionsListResponse {
  questions: InterviewQuestion[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export const QUESTION_CATEGORY_OPTIONS: { value: QuestionCategory; label: string; description: string }[] = [
  { value: 'Behavioral', label: 'Behavioral', description: '"Tell me about a time..."' },
  { value: 'Technical', label: 'Technical', description: 'Skills, tools, coding' },
  { value: 'Situational', label: 'Situational', description: '"What would you do if..."' },
  { value: 'CompanySpecific', label: 'Company Specific', description: 'About the company/role' },
  { value: 'General', label: 'General', description: 'Salary, availability, etc.' },
  { value: 'Other', label: 'Other', description: 'Uncategorized questions' },
];

export const getCategoryLabel = (category: QuestionCategory): string => {
  return QUESTION_CATEGORY_OPTIONS.find(opt => opt.value === category)?.label || category;
};

export const getCategoryColor = (category: QuestionCategory): string => {
  switch (category) {
    case 'Behavioral':
      return 'bg-blue-500/20 text-blue-400 border-blue-500/30';
    case 'Technical':
      return 'bg-purple-500/20 text-purple-400 border-purple-500/30';
    case 'Situational':
      return 'bg-green-500/20 text-green-400 border-green-500/30';
    case 'CompanySpecific':
      return 'bg-amber-500/20 text-amber-400 border-amber-500/30';
    case 'General':
      return 'bg-gray-500/20 text-gray-400 border-gray-500/30';
    case 'Other':
    default:
      return 'bg-gray-600/20 text-gray-300 border-gray-600/30';
  }
};
