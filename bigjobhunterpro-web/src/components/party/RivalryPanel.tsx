import type { RivalryData } from '@/types/huntingParty';

interface RivalryPanelProps {
  rivalry: RivalryData | null;
  isLoading?: boolean;
}

export default function RivalryPanel({ rivalry, isLoading }: RivalryPanelProps) {
  if (isLoading) {
    return (
      <div className="metal-panel metal-panel-orange">
        <div className="metal-panel-screws" />
        <h3 className="font-arcade text-lg text-blaze mb-4">Your Rivals</h3>
        <div className="h-24 bg-slate-800/50 rounded-lg animate-pulse" />
      </div>
    );
  }

  if (!rivalry) {
    return null;
  }

  const isFirst = rivalry.currentRank === 1;
  const isLast = rivalry.currentRank === rivalry.totalMembers;

  return (
    <div className="metal-panel metal-panel-orange">
      <div className="metal-panel-screws" />

      {/* Current Position */}
      <div className="text-center mb-4">
        <h3 className="font-arcade text-lg text-blaze mb-1">Your Position</h3>
        <div className="font-arcade text-3xl text-amber">
          #{rivalry.currentRank}
          <span className="text-sm text-gray-500 ml-1">of {rivalry.totalMembers}</span>
        </div>
      </div>

      {/* Victory Message when in first place */}
      {isFirst && (
        <div className="text-center p-4 bg-gradient-to-r from-yellow-600/20 to-yellow-500/10 border border-yellow-500/30 rounded-lg mb-4">
          <div className="font-arcade text-yellow-400 mb-1">VICTORY!</div>
          <p className="text-sm text-yellow-300/80">You're leading the pack!</p>
          {rivalry.userBehind && (
            <p className="text-xs text-gray-400 mt-2">
              {rivalry.userBehind.displayName} is {rivalry.userBehind.gap} pts behind
            </p>
          )}
        </div>
      )}

      {/* Rival Ahead */}
      {rivalry.userAhead && (
        <div className="mb-3 p-3 bg-red-900/20 border border-red-800/50 rounded-lg">
          <div className="flex items-center justify-between mb-2">
            <span className="text-xs text-gray-500 uppercase">Hunter Ahead</span>
            <span className="font-arcade text-sm text-red-400">+{rivalry.userAhead.gap} pts</span>
          </div>
          <div className="flex items-center justify-between">
            <span className="text-white font-medium">{rivalry.userAhead.displayName}</span>
            <span className="font-arcade text-amber">{rivalry.userAhead.points} pts</span>
          </div>
          <div className="mt-2">
            <p className="text-xs text-blaze">Close the gap! Log an application!</p>
          </div>
        </div>
      )}

      {/* Rival Behind */}
      {rivalry.userBehind && !isFirst && (
        <div className="p-3 bg-green-900/20 border border-green-800/50 rounded-lg">
          <div className="flex items-center justify-between mb-2">
            <span className="text-xs text-gray-500 uppercase">Hunter Behind</span>
            <span className="font-arcade text-sm text-terminal">-{rivalry.userBehind.gap} pts</span>
          </div>
          <div className="flex items-center justify-between">
            <span className="text-white font-medium">{rivalry.userBehind.displayName}</span>
            <span className="font-arcade text-amber">{rivalry.userBehind.points} pts</span>
          </div>
          {rivalry.userBehind.gap <= 5 && (
            <div className="mt-2">
              <p className="text-xs text-yellow-400">They're gaining on you!</p>
            </div>
          )}
        </div>
      )}

      {/* Last Place Encouragement */}
      {isLast && rivalry.totalMembers > 1 && (
        <div className="text-center p-4 bg-slate-800/50 border border-slate-600 rounded-lg">
          <p className="text-gray-400 text-sm mb-1">Don't give up!</p>
          <p className="text-xs text-gray-500">Every application counts. Start climbing!</p>
        </div>
      )}

      {/* Single Member */}
      {rivalry.totalMembers === 1 && (
        <div className="text-center p-4 bg-slate-800/50 border border-slate-600 rounded-lg">
          <p className="text-gray-400 text-sm mb-1">You're the only hunter!</p>
          <p className="text-xs text-gray-500">Invite friends to compete</p>
        </div>
      )}
    </div>
  );
}
