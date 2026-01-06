interface EmptyStateProps {
  onQuickCapture: () => void;
}

export default function EmptyState({ onQuickCapture }: EmptyStateProps) {
  return (
    <div className="text-center py-12">
      <div className="mx-auto mb-4 h-14 w-14 rounded-full border border-metal-border flex items-center justify-center bg-metal">
        <svg
          viewBox="0 0 48 48"
          className="h-8 w-8 text-amber"
          aria-hidden="true"
        >
          <circle cx="24" cy="24" r="10" fill="none" stroke="currentColor" strokeWidth="2" />
          <circle cx="24" cy="24" r="2" fill="currentColor" />
          <line x1="24" y1="6" x2="24" y2="14" stroke="currentColor" strokeWidth="2" />
          <line x1="24" y1="34" x2="24" y2="42" stroke="currentColor" strokeWidth="2" />
          <line x1="6" y1="24" x2="14" y2="24" stroke="currentColor" strokeWidth="2" />
          <line x1="34" y1="24" x2="42" y2="24" stroke="currentColor" strokeWidth="2" />
        </svg>
      </div>
      <h3 className="font-arcade text-base text-amber mb-2">
        NO HUNTS LOGGED YET
      </h3>
      <p className="text-gray-400 mb-6">
        Log your first application to start tracking your progress.
      </p>
      <button className="btn-metal-primary" onClick={onQuickCapture}>
        LOG YOUR FIRST HUNT
      </button>
    </div>
  );
}
