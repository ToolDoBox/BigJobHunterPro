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
      <div className="min-h-screen bg-metal-dark app-shell flex items-center justify-center">
        <div className="metal-panel text-center">
          <div className="metal-panel-screws" />
          <h2 className="font-arcade text-base text-amber mb-4">
            LOADING...
          </h2>
          <div className="stat-display w-48 mx-auto overflow-hidden">
            <div className="h-3 bg-terminal animate-pulse w-2/3" />
          </div>
          <p className="font-arcade text-xs text-terminal mt-4">
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
