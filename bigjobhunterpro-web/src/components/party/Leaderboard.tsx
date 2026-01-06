import type { LeaderboardEntry } from '@/types/huntingParty';
import LeaderboardRow from './LeaderboardRow';

interface LeaderboardProps {
  entries: LeaderboardEntry[];
  isLoading?: boolean;
  currentUserId?: string;
}

export default function Leaderboard({ entries, isLoading, currentUserId }: LeaderboardProps) {
  if (isLoading) {
    return (
      <div className="metal-panel metal-panel-green">
        <div className="metal-panel-screws" />
        <h3 className="font-arcade text-lg text-terminal mb-4">Leaderboard</h3>
        <div className="space-y-2">
          {[1, 2, 3, 4, 5].map((i) => (
            <div
              key={i}
              className="h-12 bg-slate-800/50 rounded-lg animate-pulse"
            />
          ))}
        </div>
      </div>
    );
  }

  if (entries.length === 0) {
    return (
      <div className="metal-panel metal-panel-green">
        <div className="metal-panel-screws" />
        <h3 className="font-arcade text-lg text-terminal mb-4">Leaderboard</h3>
        <p className="text-gray-400 text-center py-8">
          No hunters yet. Be the first to log an application!
        </p>
      </div>
    );
  }

  // Update isCurrentUser based on currentUserId prop
  const entriesWithCurrentUser = entries.map((entry) => ({
    ...entry,
    isCurrentUser: currentUserId ? entry.userId === currentUserId : entry.isCurrentUser,
  }));

  return (
    <div className="metal-panel metal-panel-green">
      <div className="metal-panel-screws" />

      <div className="flex items-center justify-between mb-4">
        <h3 className="font-arcade text-lg text-terminal">Leaderboard</h3>
        <span className="text-xs text-gray-500">Real-time updates</span>
      </div>

      <div className="space-y-2">
        {entriesWithCurrentUser.map((entry) => (
          <LeaderboardRow key={entry.userId} entry={entry} />
        ))}
      </div>
    </div>
  );
}
