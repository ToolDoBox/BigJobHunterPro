import type { InterviewQuestion } from '@/types/question';
import QuestionCard from './QuestionCard';

interface QuestionsListProps {
  questions: InterviewQuestion[];
  onEdit: (question: InterviewQuestion) => void;
  onDelete: (question: InterviewQuestion) => void;
  onIncrementAsked: (question: InterviewQuestion) => void;
}

export default function QuestionsList({
  questions,
  onEdit,
  onDelete,
  onIncrementAsked,
}: QuestionsListProps) {
  if (questions.length === 0) {
    return null;
  }

  return (
    <div className="space-y-3">
      {questions.map((question) => (
        <QuestionCard
          key={question.id}
          question={question}
          onEdit={onEdit}
          onDelete={onDelete}
          onIncrementAsked={onIncrementAsked}
        />
      ))}
    </div>
  );
}
