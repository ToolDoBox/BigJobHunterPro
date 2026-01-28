import type { InterviewQuestion } from '@/types/question';
import CategoryBadge from './CategoryBadge';

interface QuestionCardProps {
  question: InterviewQuestion;
  onEdit: (question: InterviewQuestion) => void;
  onDelete: (question: InterviewQuestion) => void;
  onIncrementAsked: (question: InterviewQuestion) => void;
}

export default function QuestionCard({
  question,
  onEdit,
  onDelete,
  onIncrementAsked,
}: QuestionCardProps) {
  const formatDate = (dateString: string | null) => {
    if (!dateString) return null;
    return new Date(dateString).toLocaleDateString(undefined, {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
    });
  };

  return (
    <div className="metal-panel p-4 hover:border-amber/30 transition-colors">
      <div className="flex flex-col gap-3">
        {/* Header */}
        <div className="flex items-start justify-between gap-2">
          <div className="flex-1 min-w-0">
            <h3 className="text-amber font-medium text-sm md:text-base leading-relaxed">
              {question.questionText}
            </h3>
          </div>
          <CategoryBadge category={question.category} className="flex-shrink-0" />
        </div>

        {/* Answer Preview */}
        {question.answerText && (
          <div className="text-gray-400 text-sm line-clamp-2 pl-2 border-l-2 border-terminal/30">
            {question.answerText}
          </div>
        )}

        {/* Tags */}
        {question.tags.length > 0 && (
          <div className="flex flex-wrap gap-1">
            {question.tags.map((tag, index) => (
              <span
                key={index}
                className="inline-flex items-center px-2 py-0.5 rounded text-xs bg-metal-light text-gray-400"
              >
                #{tag}
              </span>
            ))}
          </div>
        )}

        {/* Metadata and Actions */}
        <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-2 pt-2 border-t border-metal-border">
          <div className="flex flex-wrap items-center gap-3 text-xs text-gray-500">
            <span className="flex items-center gap-1">
              <span className="text-terminal">{question.timesAsked}x</span> asked
            </span>
            {question.lastAskedDate && (
              <span>Last: {formatDate(question.lastAskedDate)}</span>
            )}
            {question.applicationCompany && (
              <span className="text-blaze/70">
                {question.applicationCompany}
                {question.applicationRole && ` - ${question.applicationRole}`}
              </span>
            )}
          </div>

          <div className="flex items-center gap-1 sm:gap-2">
            <button
              onClick={() => onIncrementAsked(question)}
              className="btn-metal text-xs px-2 py-1"
              title="Asked again"
            >
              +1 ASKED
            </button>
            <button
              onClick={() => onEdit(question)}
              className="btn-metal text-xs px-2 py-1"
            >
              EDIT
            </button>
            <button
              onClick={() => onDelete(question)}
              className="btn-metal text-xs px-2 py-1 hover:text-red-400"
            >
              DELETE
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
