import { createBrowserRouter, Navigate } from 'react-router-dom';
import AppShell from '@/components/layout/AppShell';
import ProtectedRoute from './ProtectedRoute';
import Login from '@/pages/Login';
import Register from '@/pages/Register';
import Dashboard from '@/pages/Dashboard';

export const router = createBrowserRouter([
  // Public routes
  {
    path: '/login',
    element: <Login />,
  },
  {
    path: '/register',
    element: <Register />,
  },

  // Protected routes (require authentication)
  {
    path: '/app',
    element: (
      <ProtectedRoute>
        <AppShell />
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        element: <Navigate to="dashboard" replace />,
      },
      {
        path: 'dashboard',
        element: <Dashboard />,
      },
      // Placeholder routes for future sprints
      {
        path: 'applications',
        element: (
          <div className="metal-panel text-center">
            <div className="metal-panel-screws" />
            <h2 className="font-arcade text-xl text-amber mb-4">
              THE ARMORY
            </h2>
            <p className="text-gray-400">Coming in Story 3...</p>
          </div>
        ),
      },
      {
        path: 'party',
        element: (
          <div className="metal-panel text-center">
            <div className="metal-panel-screws" />
            <h2 className="font-arcade text-xl text-amber mb-4">
              HUNTING PARTY
            </h2>
            <p className="text-gray-400">Coming in future sprint...</p>
          </div>
        ),
      },
      {
        path: 'profile',
        element: (
          <div className="metal-panel text-center">
            <div className="metal-panel-screws" />
            <h2 className="font-arcade text-xl text-amber mb-4">
              HUNTER PROFILE
            </h2>
            <p className="text-gray-400">Coming in future sprint...</p>
          </div>
        ),
      },
    ],
  },

  // Catch-all redirect
  {
    path: '*',
    element: <Navigate to="/app" replace />,
  },
]);
