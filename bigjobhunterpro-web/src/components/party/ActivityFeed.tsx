import { HubConnectionState } from '@microsoft/signalr';
import ActivityEventCard from './ActivityEventCard';
import type { ActivityEvent } from '@/types/activity';

interface ActivityFeedProps {
  events: ActivityEvent[];
  isLoading?: boolean;
  connectionState: HubConnectionState;
  hasMore?: boolean;
}

export default function ActivityFeed({
  events,
  isLoading,
  connectionState,
  hasMore,
}: ActivityFeedProps) {
  const isReconnecting = connectionState === HubConnectionState.Reconnecting;
  const isDisconnected = connectionState === HubConnectionState.Disconnected;

  return (
    <div className="metal-panel metal-panel-orange">
      <div className="metal-panel-screws" />
      <div className="flex items-center justify-between mb-4">
        <h3 className="font-arcade text-lg text-amber">Party Activity</h3>
        <span className="text-xs text-gray-500">Latest updates</span>
      </div>

      {(isReconnecting || isDisconnected) && (
        <div className="mb-3 rounded-md border border-amber/40 bg-amber/10 px-3 py-2 text-xs text-amber">
          Live updates paused. We will reconnect in the background.
        </div>
      )}

      {isLoading ? (
        <div className="space-y-3">
          {[1, 2, 3, 4].map((i) => (
            <div key={i} className="h-16 rounded-lg bg-slate-800/50 animate-pulse" />
          ))}
        </div>
      ) : events.length === 0 ? (
        <div className="rounded-lg border border-slate-700 bg-slate-900/60 px-4 py-6 text-center">
          <div className="text-2xl mb-2">🚀</div>
          <p className="text-gray-300">
            No activity yet. Log an application or update a status to fire up the feed.
          </p>
        </div>
      ) : (
        <div>
          <ul className="space-y-3" role="list">
            {events.map((event) => (
              <li key={event.id}>
                <ActivityEventCard event={event} />
              </li>
            ))}
          </ul>
          {hasMore && (
            <div className="mt-3 text-xs text-gray-500">
              Showing the latest activity. Load more in a future sprint.
            </div>
          )}
        </div>
      )}
    </div>
  );
}
