import { useToast } from '@/context/ToastContext';
import Toast from './Toast';

export default function ToastContainer() {
  const { toasts, dismissToast } = useToast();

  if (toasts.length === 0) return null;

  return (
    <div className="fixed top-4 left-4 right-4 z-50 space-y-2 sm:left-auto sm:right-4">
      {toasts.map(toast => (
        <Toast
          key={toast.id}
          type={toast.type}
          message={toast.message}
          points={toast.points}
          onDismiss={() => dismissToast(toast.id)}
        />
      ))}
    </div>
  );
}
