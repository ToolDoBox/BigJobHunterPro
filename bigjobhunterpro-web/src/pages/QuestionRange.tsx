import { useCallback, useEffect, useState } from 'react';
import QuestionsList from '@/components/questions/QuestionsList';
import AddQuestionModal from '@/components/questions/AddQuestionModal';
import EditQuestionModal from '@/components/questions/EditQuestionModal';
import { questionsService } from '@/services/questions';
import type {
  InterviewQuestion,
  QuestionCategory,
  CreateInterviewQuestionRequest,
  UpdateInterviewQuestionRequest,
} from '@/types/question';
import { QUESTION_CATEGORY_OPTIONS } from '@/types/question';
import { useDebounce } from '@/hooks/useDebounce';
import { useKeyboardShortcut } from '@/hooks/useKeyboardShortcut';

const PAGE_SIZE = 20;

const SORT_OPTIONS = [
  { value: 'createdDate', label: 'Newest First' },
  { value: 'timesAsked', label: 'Most Asked' },
  { value: 'lastAskedDate', label: 'Recently Asked' },
  { value: 'questionText', label: 'Alphabetical' },
];

export default function QuestionRange() {
  const [questions, setQuestions] = useState<InterviewQuestion[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isLoadingMore, setIsLoadingMore] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [hasMore, setHasMore] = useState(false);

  // Filters
  const [searchTerm, setSearchTerm] = useState('');
  const [categoryFilter, setCategoryFilter] = useState<QuestionCategory | ''>('');
  const [sortBy, setSortBy] = useState('createdDate');

  // Modals
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [editingQuestion, setEditingQuestion] = useState<InterviewQuestion | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Delete confirmation
  const [deletingQuestion, setDeletingQuestion] = useState<InterviewQuestion | null>(null);

  const debouncedSearch = useDebounce(searchTerm, 300);

  const fetchQuestions = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const response = await questionsService.getAll({
        category: categoryFilter || undefined,
        search: debouncedSearch || undefined,
        page: 1,
        pageSize: PAGE_SIZE,
        sortBy,
        sortDescending: sortBy !== 'questionText',
      });
      setQuestions(response.questions);
      setTotalCount(response.totalCount);
      setPage(response.page);
      setHasMore(response.page < response.totalPages);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Unable to load questions.';
      setError(message);
    } finally {
      setIsLoading(false);
    }
  }, [debouncedSearch, categoryFilter, sortBy]);

  const loadMore = useCallback(async () => {
    if (isLoadingMore || !hasMore) return;

    setIsLoadingMore(true);
    setError(null);

    try {
      const response = await questionsService.getAll({
        category: categoryFilter || undefined,
        search: debouncedSearch || undefined,
        page: page + 1,
        pageSize: PAGE_SIZE,
        sortBy,
        sortDescending: sortBy !== 'questionText',
      });
      setQuestions((prev) => [...prev, ...response.questions]);
      setPage(response.page);
      setHasMore(response.page < response.totalPages);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Unable to load more questions.';
      setError(message);
    } finally {
      setIsLoadingMore(false);
    }
  }, [hasMore, isLoadingMore, page, debouncedSearch, categoryFilter, sortBy]);

  // Fetch on initial load and when filters change
  useEffect(() => {
    fetchQuestions();
  }, [fetchQuestions]);

  // Keyboard shortcut to open add modal
  useKeyboardShortcut('k', () => setIsAddModalOpen(true), {
    ctrl: true,
    disabled: isAddModalOpen || isEditModalOpen,
  });

  const handleClearFilters = () => {
    setSearchTerm('');
    setCategoryFilter('');
    setSortBy('createdDate');
  };

  const hasActiveFilters = searchTerm.trim() !== '' || categoryFilter !== '' || sortBy !== 'createdDate';

  const handleAddQuestion = async (data: CreateInterviewQuestionRequest) => {
    setIsSubmitting(true);
    try {
      await questionsService.create(data);
      setIsAddModalOpen(false);
      await fetchQuestions();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to add question.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleEditQuestion = (question: InterviewQuestion) => {
    setEditingQuestion(question);
    setIsEditModalOpen(true);
  };

  const handleUpdateQuestion = async (id: string, data: UpdateInterviewQuestionRequest) => {
    setIsSubmitting(true);
    try {
      await questionsService.update(id, data);
      setIsEditModalOpen(false);
      setEditingQuestion(null);
      await fetchQuestions();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update question.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeleteQuestion = (question: InterviewQuestion) => {
    setDeletingQuestion(question);
  };

  const confirmDelete = async () => {
    if (!deletingQuestion) return;

    setIsSubmitting(true);
    try {
      await questionsService.delete(deletingQuestion.id);
      setDeletingQuestion(null);
      await fetchQuestions();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete question.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleIncrementAsked = async (question: InterviewQuestion) => {
    try {
      const updated = await questionsService.incrementTimesAsked(question.id);
      setQuestions((prev) =>
        prev.map((q) => (q.id === updated.id ? updated : q))
      );
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update question.');
    }
  };

  return (
    <div className="space-y-4 md:space-y-6">
      {/* Header */}
      <div className="metal-panel metal-panel-orange p-3 sm:p-4 md:p-6">
        <div className="metal-panel-screws" />
        <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
          <div className="min-w-0">
            <h1 className="font-arcade text-base md:text-xl lg:text-2xl text-blaze mb-2 leading-relaxed">
              QUESTION RANGE
            </h1>
            <p className="text-sm md:text-base text-gray-400">
              Practice your aim. Collect questions, craft answers, and hit the target every time.
            </p>
          </div>
          <button
            className="btn-metal-primary w-full md:w-auto shrink-0"
            onClick={() => setIsAddModalOpen(true)}
          >
            + ADD QUESTION
          </button>
        </div>
      </div>

      {/* Questions List Panel */}
      <div className="metal-panel p-3 sm:p-4 md:p-6">
        <div className="metal-panel-screws" />
        <div className="flex flex-col gap-2 md:flex-row md:items-center md:justify-between mb-4">
          <h2 className="font-arcade text-xs md:text-sm text-amber leading-relaxed">QUESTION BANK</h2>
          <span className="text-xs text-gray-500">{totalCount} questions</span>
        </div>

        {/* Search and Filter Controls */}
        <div className="flex flex-col gap-3 mb-4">
          <div className="w-full">
            <input
              type="text"
              placeholder="Search questions..."
              className="input-arcade w-full text-sm md:text-base"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
          <div className="flex flex-col sm:flex-row gap-2">
            <select
              className="input-arcade flex-1 text-sm md:text-base"
              value={categoryFilter}
              onChange={(e) => setCategoryFilter(e.target.value as QuestionCategory | '')}
            >
              <option value="">All Categories</option>
              {QUESTION_CATEGORY_OPTIONS.map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
            <select
              className="input-arcade flex-1 text-sm md:text-base"
              value={sortBy}
              onChange={(e) => setSortBy(e.target.value)}
            >
              {SORT_OPTIONS.map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
            {hasActiveFilters && (
              <button
                className="btn-metal w-full sm:w-auto"
                onClick={handleClearFilters}
                title="Clear filters"
              >
                CLEAR
              </button>
            )}
          </div>
        </div>

        {/* Content */}
        {isLoading ? (
          <div className="flex items-center justify-center py-12">
            <div className="text-center">
              <div className="text-4xl mb-4 animate-pulse">🎯</div>
              <p className="text-gray-400">Loading questions...</p>
            </div>
          </div>
        ) : error && questions.length === 0 ? (
          <div className="animate-fade-in motion-reduce:animate-none">
            <div className="text-center py-12">
              <p className="text-red-400 mb-4">{error}</p>
              <button className="btn-metal" onClick={fetchQuestions}>
                TRY AGAIN
              </button>
            </div>
          </div>
        ) : questions.length === 0 ? (
          <div className="animate-fade-in motion-reduce:animate-none">
            <div className="text-center py-12">
              <div className="text-6xl mb-4">🎯</div>
              <h3 className="font-arcade text-sm text-amber mb-2">NO QUESTIONS YET</h3>
              <p className="text-gray-400 mb-6 max-w-md mx-auto">
                Start building your interview prep arsenal. Add questions you've been asked
                or want to practice for.
              </p>
              <button
                className="btn-metal-primary"
                onClick={() => setIsAddModalOpen(true)}
              >
                + ADD YOUR FIRST QUESTION
              </button>
            </div>
          </div>
        ) : (
          <div className="animate-fade-in motion-reduce:animate-none">
            <QuestionsList
              questions={questions}
              onEdit={handleEditQuestion}
              onDelete={handleDeleteQuestion}
              onIncrementAsked={handleIncrementAsked}
            />
            {error && (
              <div className="mt-4 text-center text-sm text-red-400">{error}</div>
            )}
            {hasMore && (
              <div className="mt-6 flex justify-center">
                <button
                  className="btn-metal"
                  onClick={loadMore}
                  disabled={isLoadingMore}
                >
                  {isLoadingMore ? 'LOADING...' : 'LOAD MORE'}
                </button>
              </div>
            )}
          </div>
        )}
      </div>

      {/* Add Question Modal */}
      <AddQuestionModal
        isOpen={isAddModalOpen}
        onClose={() => setIsAddModalOpen(false)}
        onSubmit={handleAddQuestion}
        isSubmitting={isSubmitting}
      />

      {/* Edit Question Modal */}
      <EditQuestionModal
        isOpen={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setEditingQuestion(null);
        }}
        onSubmit={handleUpdateQuestion}
        question={editingQuestion}
        isSubmitting={isSubmitting}
      />

      {/* Delete Confirmation Dialog */}
      {deletingQuestion && (
        <div className="fixed inset-0 z-50 flex items-center justify-center px-4">
          <div
            className="absolute inset-0 bg-black/70 backdrop-blur-sm animate-fade-in"
            onClick={() => setDeletingQuestion(null)}
          />
          <div className="relative metal-panel metal-panel-orange w-full max-w-md p-6 animate-scale-in">
            <div className="metal-panel-screws" />
            <h3 className="font-arcade text-lg text-blaze mb-4">DELETE QUESTION?</h3>
            <p className="text-gray-400 mb-2">Are you sure you want to delete this question?</p>
            <p className="text-amber text-sm mb-6 line-clamp-2">
              "{deletingQuestion.questionText}"
            </p>
            <div className="flex gap-3 justify-end">
              <button
                className="btn-metal"
                onClick={() => setDeletingQuestion(null)}
                disabled={isSubmitting}
              >
                Cancel
              </button>
              <button
                className="btn-metal hover:bg-red-500/20 hover:border-red-500/50 hover:text-red-400"
                onClick={confirmDelete}
                disabled={isSubmitting}
              >
                {isSubmitting ? 'Deleting...' : 'Delete'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
