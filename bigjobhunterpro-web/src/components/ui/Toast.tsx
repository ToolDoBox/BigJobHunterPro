import type { ToastType } from '@/context/ToastContext';

interface ToastProps {
  type: ToastType;
  message: string;
  points?: number;
  onDismiss: () => void;
}

export default function Toast({ type, message, points, onDismiss }: ToastProps) {
  const borderColor = type === 'success'
    ? 'border-terminal'
    : type === 'error'
    ? 'border-red-500'
    : 'border-amber';

  return (
    <div
      className={`metal-panel ${borderColor} border-l-4 flex items-center gap-4 w-full max-w-full sm:min-w-[300px] sm:w-auto animate-slide-in-right`}
    >
      <div className="flex-1">
        <p className="text-white font-medium">{message}</p>
        {points !== undefined && (
          <p className="font-arcade text-terminal text-sm mt-1">
            +{points} POINT{points !== 1 ? 'S' : ''}!
          </p>
        )}
      </div>
      <button
        onClick={onDismiss}
        className="text-gray-400 hover:text-white font-arcade text-xs"
        aria-label="Dismiss"
      >
        X
      </button>
    </div>
  );
}
