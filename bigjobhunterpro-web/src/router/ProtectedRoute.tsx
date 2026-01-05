import type { ReactNode } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '@/hooks/useAuth';

interface ProtectedRouteProps {
  children: ReactNode;
}

export default function ProtectedRoute({ children }: ProtectedRouteProps) {
  const { isAuthenticated, isLoading } = useAuth();
  const location = useLocation();

  // Show loading state while checking authentication
  if (isLoading) {
    return (
      <div className="min-h-screen bg-gradient-arcade flex items-center justify-center">
        <div className="text-center">
          <div className="scanlines" />
          <h2 className="font-arcade text-lg text-amber text-glow-amber mb-4">
            LOADING...
          </h2>
          <div className="w-48 h-4 bg-charcoal border-2 border-terminal overflow-hidden mx-auto">
            <div className="h-full bg-terminal animate-pulse w-2/3" />
          </div>
          <p className="font-arcade text-xs text-terminal mt-4 animate-blink">
            Initializing hunt systems...
          </p>
        </div>
      </div>
    );
  }

  // Redirect to login if not authenticated
  if (!isAuthenticated) {
    // Save the attempted URL for redirecting after login
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <>{children}</>;
}
