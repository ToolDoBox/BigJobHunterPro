interface ErrorStateProps {
  message: string;
  onRetry: () => void;
}

export default function ErrorState({ message, onRetry }: ErrorStateProps) {
  return (
    <div className="text-center py-12">
      <h3 className="font-arcade text-base text-blaze mb-2">
        FAILED TO LOAD
      </h3>
      <p className="text-gray-400 mb-6">{message}</p>
      <button className="btn-metal" onClick={onRetry}>
        RETRY
      </button>
    </div>
  );
}
