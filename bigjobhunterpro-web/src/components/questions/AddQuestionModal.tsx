import { useState } from 'react';
import SidePanel from '../ui/SidePanel';
import type { QuestionCategory, CreateInterviewQuestionRequest } from '../../types/question';
import { QUESTION_CATEGORY_OPTIONS } from '../../types/question';

interface AddQuestionModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: CreateInterviewQuestionRequest) => void;
  isSubmitting?: boolean;
}

export default function AddQuestionModal({
  isOpen,
  onClose,
  onSubmit,
  isSubmitting = false,
}: AddQuestionModalProps) {
  const [questionText, setQuestionText] = useState('');
  const [answerText, setAnswerText] = useState('');
  const [notes, setNotes] = useState('');
  const [category, setCategory] = useState<QuestionCategory>('Behavioral');
  const [tagsText, setTagsText] = useState('');
  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const newErrors: Record<string, string> = {};
    if (!questionText.trim()) newErrors.questionText = 'Question text is required';

    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }

    // Parse tags from comma-separated text
    const tags = tagsText
      .split(',')
      .map((t) => t.trim().toLowerCase())
      .filter((t) => t.length > 0);

    onSubmit({
      questionText: questionText.trim(),
      answerText: answerText.trim() || undefined,
      notes: notes.trim() || undefined,
      category,
      tags,
    });

    // Reset form
    setQuestionText('');
    setAnswerText('');
    setNotes('');
    setCategory('Behavioral');
    setTagsText('');
    setErrors({});
  };

  return (
    <SidePanel isOpen={isOpen} onClose={onClose} title="Add Question">
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label htmlFor="questionText" className="block text-sm font-semibold text-gray-300 mb-2">
            Question *
          </label>
          <textarea
            id="questionText"
            rows={3}
            value={questionText}
            onChange={(e) => setQuestionText(e.target.value)}
            className={`input-arcade ${errors.questionText ? 'input-arcade-error' : ''}`}
            placeholder='e.g., "Tell me about a time you had to deal with a difficult stakeholder"'
            maxLength={2000}
          />
          {errors.questionText && <p className="text-red text-sm mt-1">{errors.questionText}</p>}
          <div className="text-xs text-gray-400 mt-1">{questionText.length}/2000 characters</div>
        </div>

        <div>
          <label htmlFor="category" className="block text-sm font-semibold text-gray-300 mb-2">
            Category
          </label>
          <select
            id="category"
            value={category}
            onChange={(e) => setCategory(e.target.value as QuestionCategory)}
            className="input-arcade"
          >
            {QUESTION_CATEGORY_OPTIONS.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label} - {option.description}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label htmlFor="answerText" className="block text-sm font-semibold text-gray-300 mb-2">
            Your Answer
          </label>
          <textarea
            id="answerText"
            rows={5}
            maxLength={5000}
            value={answerText}
            onChange={(e) => setAnswerText(e.target.value)}
            className="input-arcade"
            placeholder="Write your ideal answer to practice..."
          />
          <div className="text-xs text-gray-400 mt-1">{answerText.length}/5000 characters</div>
        </div>

        <div>
          <label htmlFor="tags" className="block text-sm font-semibold text-gray-300 mb-2">
            Tags
          </label>
          <input
            type="text"
            id="tags"
            value={tagsText}
            onChange={(e) => setTagsText(e.target.value)}
            className="input-arcade"
            placeholder="e.g., leadership, conflict, communication"
          />
          <p className="text-xs text-gray-400 mt-1">Separate multiple tags with commas</p>
        </div>

        <div>
          <label htmlFor="notes" className="block text-sm font-semibold text-gray-300 mb-2">
            Notes
          </label>
          <textarea
            id="notes"
            rows={3}
            maxLength={2000}
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            className="input-arcade"
            placeholder="Any additional notes, tips, or context..."
          />
          <div className="text-xs text-gray-400 mt-1">{notes.length}/2000 characters</div>
        </div>

        <div className="flex gap-3 justify-end pt-4">
          <button type="button" onClick={onClose} className="btn-metal" disabled={isSubmitting}>
            Cancel
          </button>
          <button type="submit" className="btn-metal-primary" disabled={isSubmitting}>
            {isSubmitting ? 'Adding...' : 'Add Question'}
          </button>
        </div>
      </form>
    </SidePanel>
  );
}
