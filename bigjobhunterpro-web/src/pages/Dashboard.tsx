import { useAuth } from '@/hooks/useAuth';

export default function Dashboard() {
  const { user } = useAuth();

  return (
    <div className="space-y-8">
      {/* Welcome banner - Metal panel with orange border */}
      <div className="metal-panel metal-panel-orange">
        <div className="metal-panel-screws" />
        <h1 className="font-arcade text-xl md:text-2xl text-blaze mb-2">
          WELCOME TO THE LODGE
        </h1>
        <p className="text-amber font-semibold">
          Welcome back, {user?.displayName ?? 'Hunter'}!
        </p>
        <p className="text-gray-400 mt-2">
          Your personal hunting command center. Track applications, earn points, and climb the leaderboards.
        </p>
      </div>

      {/* Stats - Industrial gauge/readout style */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        {/* Total Points */}
        <div className="metal-panel text-center">
          <div className="metal-panel-screws" />
          <div className="stat-display inline-block mb-3">
            <span className="stat-display-value text-3xl">
              {user?.points ?? 0}
            </span>
          </div>
          <div className="font-arcade text-xs text-gray-500 tracking-wider">
            TOTAL POINTS
          </div>
        </div>

        {/* Applications */}
        <div className="metal-panel text-center">
          <div className="metal-panel-screws" />
          <div className="stat-display inline-block mb-3">
            <span className="stat-display-value text-3xl text-amber">0</span>
          </div>
          <div className="font-arcade text-xs text-gray-500 tracking-wider">
            APPLICATIONS
          </div>
        </div>

        {/* Day Streak */}
        <div className="metal-panel text-center">
          <div className="metal-panel-screws" />
          <div className="stat-display inline-block mb-3">
            <span className="stat-display-value text-3xl text-blaze">0</span>
          </div>
          <div className="font-arcade text-xs text-gray-500 tracking-wider">
            DAY STREAK
          </div>
        </div>
      </div>

      {/* Quick actions - Metal panel */}
      <div className="metal-panel">
        <div className="metal-panel-screws" />
        <h2 className="font-arcade text-base text-amber mb-4">QUICK ACTIONS</h2>
        <button className="btn-metal-primary">
          + QUICK CAPTURE
        </button>
        <p className="text-gray-500 text-sm mt-4">
          Quick Capture coming in Sprint 1 Story 2...
        </p>
      </div>
    </div>
  );
}
