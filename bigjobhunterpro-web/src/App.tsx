import { useEffect } from 'react';
import { RouterProvider } from 'react-router-dom';
import { AuthProvider } from '@/context/AuthContext';
import { ToastProvider } from '@/context/ToastContext';
import { router } from '@/router';
import { initiateWarmup } from '@/services/warmup';

function App() {
  // Warm up the API server on initial load (handles Azure cold starts)
  useEffect(() => {
    initiateWarmup();
  }, []);

  return (
    <AuthProvider>
      <ToastProvider>
        <RouterProvider router={router} />
      </ToastProvider>
    </AuthProvider>
  );
}

export default App;
