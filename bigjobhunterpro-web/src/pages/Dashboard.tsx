import { useAuth } from '@/hooks/useAuth';

export default function Dashboard() {
  const { user } = useAuth();

  return (
    <div className="space-y-8">
      {/* Welcome banner */}
      <div className="bg-charcoal border-4 border-blaze p-6 shadow-orange-glow">
        <h1 className="font-arcade text-xl md:text-2xl text-blaze text-glow-blaze mb-2">
          WELCOME TO THE LODGE
        </h1>
        <p className="text-amber font-semibold">
          Welcome back, {user?.displayName ?? 'Hunter'}!
        </p>
        <p className="text-gray-400 mt-2">
          Your personal hunting command center. Track applications, earn points, and climb the leaderboards.
        </p>
      </div>

      {/* Stats placeholder */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="bg-forest/30 border-2 border-terminal p-6 text-center">
          <div className="font-arcade text-3xl text-terminal text-glow-terminal">
            {user?.points ?? 0}
          </div>
          <div className="font-arcade text-xs text-gray-400 mt-2">TOTAL POINTS</div>
        </div>

        <div className="bg-forest/30 border-2 border-amber p-6 text-center">
          <div className="font-arcade text-3xl text-amber text-glow-amber">0</div>
          <div className="font-arcade text-xs text-gray-400 mt-2">APPLICATIONS</div>
        </div>

        <div className="bg-forest/30 border-2 border-blaze p-6 text-center">
          <div className="font-arcade text-3xl text-blaze text-glow-blaze">0</div>
          <div className="font-arcade text-xs text-gray-400 mt-2">DAY STREAK</div>
        </div>
      </div>

      {/* Quick actions placeholder */}
      <div className="bg-charcoal border-2 border-forest p-6">
        <h2 className="font-arcade text-lg text-amber mb-4">QUICK ACTIONS</h2>
        <button className="btn-arcade-primary">
          + QUICK CAPTURE
        </button>
        <p className="text-gray-500 text-sm mt-4">
          Quick Capture coming in Sprint 1 Story 2...
        </p>
      </div>
    </div>
  );
}
