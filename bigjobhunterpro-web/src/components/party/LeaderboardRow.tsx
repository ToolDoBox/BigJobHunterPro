import type { LeaderboardEntry } from '@/types/huntingParty';

interface LeaderboardRowProps {
  entry: LeaderboardEntry;
}

export default function LeaderboardRow({ entry }: LeaderboardRowProps) {
  const getRankStyle = (rank: number) => {
    switch (rank) {
      case 1:
        return 'bg-gradient-to-r from-yellow-600/30 to-yellow-500/10 border-yellow-500/50 text-yellow-400';
      case 2:
        return 'bg-gradient-to-r from-gray-400/30 to-gray-400/10 border-gray-400/50 text-gray-300';
      case 3:
        return 'bg-gradient-to-r from-amber-700/30 to-amber-700/10 border-amber-700/50 text-amber-600';
      default:
        return 'bg-slate-800/50 border-slate-600 text-gray-400';
    }
  };

  const getRankIcon = (rank: number) => {
    switch (rank) {
      case 1:
        return '1st';
      case 2:
        return '2nd';
      case 3:
        return '3rd';
      default:
        return `${rank}th`;
    }
  };

  return (
    <div
      className={`flex items-center p-3 rounded-lg border transition-all ${getRankStyle(entry.rank)} ${
        entry.isCurrentUser ? 'ring-2 ring-blaze ring-offset-2 ring-offset-slate-900' : ''
      }`}
    >
      {/* Rank */}
      <div className="w-12 flex-shrink-0">
        <span className="font-arcade text-sm">{getRankIcon(entry.rank)}</span>
      </div>

      {/* User Info */}
      <div className="flex-1 min-w-0">
        <div className="flex items-center gap-2">
          <span className={`font-medium truncate ${entry.isCurrentUser ? 'text-blaze' : 'text-white'}`}>
            {entry.displayName}
          </span>
          {entry.isCurrentUser && (
            <span className="px-1.5 py-0.5 text-xs bg-blaze/20 text-blaze rounded">YOU</span>
          )}
        </div>
        <div className="text-xs text-gray-500">
          {entry.applicationCount} {entry.applicationCount === 1 ? 'app' : 'apps'}
        </div>
      </div>

      {/* Points */}
      <div className="text-right flex-shrink-0">
        <span className="font-arcade text-lg text-amber">{entry.totalPoints}</span>
        <span className="text-xs text-gray-500 ml-1">pts</span>
      </div>
    </div>
  );
}
