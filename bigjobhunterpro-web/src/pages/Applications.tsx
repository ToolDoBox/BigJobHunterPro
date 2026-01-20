import { useCallback, useEffect, useState } from 'react';
import QuickCaptureModal from '@/components/applications/QuickCaptureModal';
import ApplicationsList from '@/components/applications/ApplicationsList';
import EmptyState from '@/components/applications/EmptyState';
import LoadingState from '@/components/applications/LoadingState';
import ErrorState from '@/components/applications/ErrorState';
import applicationsService from '@/services/applications';
import type { ApplicationListItem } from '@/types/application';
import { useKeyboardShortcut } from '@/hooks/useKeyboardShortcut';
import { useDebounce } from '@/hooks/useDebounce';

const STATUS_OPTIONS = [
  { value: '', label: 'All Statuses' },
  { value: 'Applied', label: 'Applied' },
  { value: 'Screening', label: 'Screening' },
  { value: 'Interview', label: 'Interview' },
  { value: 'Offer', label: 'Offer' },
  { value: 'Rejected', label: 'Rejected' },
  { value: 'Withdrawn', label: 'Withdrawn' },
];

const PAGE_SIZE = 25;

export default function Applications() {
  const [applications, setApplications] = useState<ApplicationListItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isLoadingMore, setIsLoadingMore] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(false);
  const [isQuickCaptureOpen, setIsQuickCaptureOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('');

  const debouncedSearch = useDebounce(searchTerm, 300);

  const fetchApplications = useCallback(async (search?: string, status?: string) => {
    setIsLoading(true);
    setError(null);

    try {
      const response = await applicationsService.getApplications(
        1,
        PAGE_SIZE,
        search || undefined,
        status || undefined
      );
      setApplications(response.items);
      setPage(response.page);
      setHasMore(response.hasMore);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Unable to load applications.';
      setError(message);
    } finally {
      setIsLoading(false);
    }
  }, []);

  const loadMore = useCallback(async () => {
    if (isLoadingMore || !hasMore) {
      return;
    }

    setIsLoadingMore(true);
    setError(null);

    try {
      const response = await applicationsService.getApplications(
        page + 1,
        PAGE_SIZE,
        debouncedSearch || undefined,
        statusFilter || undefined
      );
      setApplications((prev) => [...prev, ...response.items]);
      setPage(response.page);
      setHasMore(response.hasMore);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Unable to load more applications.';
      setError(message);
    } finally {
      setIsLoadingMore(false);
    }
  }, [hasMore, isLoadingMore, page, debouncedSearch, statusFilter]);

  // Fetch on initial load and when filters change
  useEffect(() => {
    fetchApplications(debouncedSearch, statusFilter);
  }, [debouncedSearch, statusFilter, fetchApplications]);

  const handleClearFilters = () => {
    setSearchTerm('');
    setStatusFilter('');
  };

  const hasActiveFilters = searchTerm.trim() !== '' || statusFilter !== '';

  useKeyboardShortcut('k', () => setIsQuickCaptureOpen(true), {
    ctrl: true,
    disabled: isQuickCaptureOpen,
  });

  return (
    <div className="space-y-6">
      <div className="metal-panel metal-panel-orange">
        <div className="metal-panel-screws" />
        <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
          <div>
            <h1 className="font-arcade text-xl md:text-2xl text-blaze mb-2">
              THE ARMORY
            </h1>
            <p className="text-gray-400">
              Review every hunt, spot your progress, and stay on target.
            </p>
          </div>
          <button
            className="btn-metal-primary"
            onClick={() => setIsQuickCaptureOpen(true)}
          >
            + QUICK CAPTURE
          </button>
        </div>
      </div>

      <div className="metal-panel">
        <div className="metal-panel-screws" />
        <div className="flex flex-col gap-2 md:flex-row md:items-center md:justify-between mb-4">
          <h2 className="font-arcade text-sm text-amber">APPLICATION LOG</h2>
          <span className="text-xs text-gray-500">
            {applications.length} logged
          </span>
        </div>

        {/* Search and Filter Controls */}
        <div className="flex flex-col gap-3 md:flex-row md:items-center mb-4">
          <div className="flex-1">
            <input
              type="text"
              placeholder="Search by company or role..."
              className="input-arcade w-full"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
          <div className="flex gap-2">
            <select
              className="input-arcade"
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
            >
              {STATUS_OPTIONS.map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
            {hasActiveFilters && (
              <button
                className="btn-metal"
                onClick={handleClearFilters}
                title="Clear filters"
              >
                CLEAR
              </button>
            )}
          </div>
        </div>

        {isLoading ? (
          <LoadingState />
        ) : error && applications.length === 0 ? (
          <div className="animate-fade-in motion-reduce:animate-none">
            <ErrorState message={error} onRetry={fetchApplications} />
          </div>
        ) : applications.length === 0 ? (
          <div className="animate-fade-in motion-reduce:animate-none">
            <EmptyState onQuickCapture={() => setIsQuickCaptureOpen(true)} />
          </div>
        ) : (
          <div className="animate-fade-in motion-reduce:animate-none">
            <ApplicationsList items={applications} />
            {error && (
              <div className="mt-4 text-center text-sm text-red-400">
                {error}
              </div>
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

      <QuickCaptureModal
        isOpen={isQuickCaptureOpen}
        onClose={() => setIsQuickCaptureOpen(false)}
        onSuccess={() => fetchApplications(debouncedSearch, statusFilter)}
      />
    </div>
  );
}
