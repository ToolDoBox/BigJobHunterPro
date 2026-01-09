import { useState, useEffect, useCallback } from 'react';
import { HubConnectionState } from '@microsoft/signalr';
import { useAuthContext } from '@/context/AuthContext';
import { useToast } from '@/context/ToastContext';
import { huntingPartiesService } from '@/services/huntingParties';
import { useSignalR } from '@/hooks/useSignalR';
import type { HuntingParty, LeaderboardEntry, RivalryData } from '@/types/huntingParty';
import type { ActivityEvent } from '@/types/activity';

import CreatePartyModal from '@/components/party/CreatePartyModal';
import JoinPartyModal from '@/components/party/JoinPartyModal';
import PartyCard from '@/components/party/PartyCard';
import Leaderboard from '@/components/party/Leaderboard';
import RivalryPanel from '@/components/party/RivalryPanel';
import ActivityFeed from '@/components/party/ActivityFeed';

export default function Party() {
  const { user } = useAuthContext();
  const { showToast } = useToast();

  const [party, setParty] = useState<HuntingParty | null>(null);
  const [leaderboard, setLeaderboard] = useState<LeaderboardEntry[]>([]);
  const [rivalry, setRivalry] = useState<RivalryData | null>(null);
  const [activityEvents, setActivityEvents] = useState<ActivityEvent[]>([]);
  const [activityHasMore, setActivityHasMore] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [isLeaderboardLoading, setIsLeaderboardLoading] = useState(false);
  const [isActivityLoading, setIsActivityLoading] = useState(false);
  const activityLimit = 50;

  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showJoinModal, setShowJoinModal] = useState(false);

  // SignalR connection for real-time updates
  const { connectionState, on, off, invoke } = useSignalR({
    hubPath: '/hubs/leaderboard',
    onConnected: () => {
      if (party) {
        invoke('JoinPartyGroup', party.id);
      }
    },
  });

  const {
    connectionState: activityConnectionState,
    on: onActivity,
    off: offActivity,
    invoke: invokeActivity,
  } = useSignalR({
    hubPath: '/hubs/activity',
    onConnected: () => {
      if (party) {
        invokeActivity('JoinPartyGroup', party.id);
      }
    },
  });

  // Fetch initial party data
  const fetchPartyData = useCallback(async () => {
    try {
      const userParty = await huntingPartiesService.getUserParty();
      setParty(userParty);

      if (userParty) {
        setIsLeaderboardLoading(true);
        setIsActivityLoading(true);
        const [leaderboardData, rivalryData, activityFeed] = await Promise.all([
          huntingPartiesService.getLeaderboard(userParty.id),
          huntingPartiesService.getRivalry(userParty.id),
          huntingPartiesService.getActivityFeed(userParty.id, activityLimit),
        ]);
        setLeaderboard(leaderboardData);
        setRivalry(rivalryData);
        setActivityEvents(activityFeed.events);
        setActivityHasMore(activityFeed.hasMore);
        setIsLeaderboardLoading(false);
        setIsActivityLoading(false);
      } else {
        setActivityEvents([]);
        setActivityHasMore(false);
      }
    } catch (error) {
      showToast('error', error instanceof Error ? error.message : 'Failed to load party data');
      setIsActivityLoading(false);
      setIsLeaderboardLoading(false);
    } finally {
      setIsLoading(false);
    }
  }, [showToast]);

  useEffect(() => {
    fetchPartyData();
  }, [fetchPartyData]);

  // Set up SignalR event listeners
  useEffect(() => {
    if (connectionState !== HubConnectionState.Connected) return;

    const handleLeaderboardUpdate = (entries: LeaderboardEntry[]) => {
      setLeaderboard(entries);
    };

    const handleRivalryUpdate = (data: { UserId: string; Rivalry: RivalryData }) => {
      if (data.UserId === user?.userId) {
        setRivalry(data.Rivalry);
      }
    };

    on('LeaderboardUpdated', handleLeaderboardUpdate);
    on('RivalryUpdated', handleRivalryUpdate);

    return () => {
      off('LeaderboardUpdated');
      off('RivalryUpdated');
    };
  }, [connectionState, on, off, user?.userId]);

  useEffect(() => {
    if (activityConnectionState !== HubConnectionState.Connected) return;

    const handleActivityEvent = (event: ActivityEvent) => {
      setActivityEvents((prev) => {
        if (prev.some((existing) => existing.id === event.id)) {
          return prev;
        }
        const updated = [event, ...prev];
        return updated.slice(0, activityLimit);
      });
    };

    onActivity('ActivityEventCreated', handleActivityEvent);

    return () => {
      offActivity('ActivityEventCreated');
    };
  }, [activityConnectionState, onActivity, offActivity, activityLimit]);

  // Join SignalR group when party changes
  useEffect(() => {
    if (party && connectionState === HubConnectionState.Connected) {
      invoke('JoinPartyGroup', party.id);
    }
  }, [party, connectionState, invoke]);

  useEffect(() => {
    if (party && activityConnectionState === HubConnectionState.Connected) {
      invokeActivity('JoinPartyGroup', party.id);
    }
  }, [party, activityConnectionState, invokeActivity]);

  const handlePartyCreated = (newParty: HuntingParty) => {
    setParty(newParty);
    setLeaderboard([]);
    setRivalry(null);
    setActivityEvents([]);
    setActivityHasMore(false);
    showToast('success', `Created party "${newParty.name}"!`);
  };

  const handlePartyJoined = (joinedParty: HuntingParty) => {
    setParty(joinedParty);
    // Fetch leaderboard for the newly joined party
    huntingPartiesService.getLeaderboard(joinedParty.id).then(setLeaderboard);
    huntingPartiesService.getRivalry(joinedParty.id).then(setRivalry);
    huntingPartiesService.getActivityFeed(joinedParty.id, activityLimit)
      .then((feed) => {
        setActivityEvents(feed.events);
        setActivityHasMore(feed.hasMore);
      });
    showToast('success', `Joined "${joinedParty.name}"!`);
  };

  const handleLeaveParty = async () => {
    if (!party) return;

    try {
      await huntingPartiesService.leaveParty(party.id);
      invoke('LeavePartyGroup', party.id);
      invokeActivity('LeavePartyGroup', party.id);
      setParty(null);
      setLeaderboard([]);
      setRivalry(null);
      setActivityEvents([]);
      setActivityHasMore(false);
      showToast('info', 'Left the party');
    } catch (error) {
      showToast('error', error instanceof Error ? error.message : 'Failed to leave party');
    }
  };

  if (isLoading) {
    return (
      <div className="max-w-4xl mx-auto">
        <div className="metal-panel text-center">
          <div className="metal-panel-screws" />
          <div className="animate-pulse">
            <div className="h-8 bg-slate-700 rounded w-48 mx-auto mb-4" />
            <div className="h-4 bg-slate-700 rounded w-64 mx-auto" />
          </div>
        </div>
      </div>
    );
  }

  // No party - show empty state
  if (!party) {
    return (
      <div className="max-w-2xl mx-auto">
        <div className="metal-panel text-center">
          <div className="metal-panel-screws" />

          <h2 className="font-arcade text-2xl text-amber mb-4">HUNTING PARTY</h2>

          <p className="text-gray-300 mb-6">
            Join forces with friends to compete, stay motivated, and track your job hunt together!
          </p>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-8">
            <div className="p-4 bg-slate-800/50 border border-slate-600 rounded-lg">
              <h3 className="font-arcade text-terminal mb-2">Create a Party</h3>
              <p className="text-sm text-gray-400 mb-3">
                Start your own party and invite friends with a unique code
              </p>
              <button
                onClick={() => setShowCreateModal(true)}
                className="w-full px-4 py-2 bg-blaze hover:bg-orange-600 text-white font-bold rounded-lg transition-colors"
              >
                Create Party
              </button>
            </div>

            <div className="p-4 bg-slate-800/50 border border-slate-600 rounded-lg">
              <h3 className="font-arcade text-terminal mb-2">Join a Party</h3>
              <p className="text-sm text-gray-400 mb-3">
                Have an invite code? Join an existing hunting party
              </p>
              <button
                onClick={() => setShowJoinModal(true)}
                className="w-full px-4 py-2 bg-slate-700 hover:bg-slate-600 text-white font-bold rounded-lg transition-colors"
              >
                Join Party
              </button>
            </div>
          </div>

          <div className="p-4 bg-forest/20 border border-forest/30 rounded-lg">
            <h4 className="text-terminal font-bold mb-2">Why Join a Party?</h4>
            <ul className="text-sm text-gray-400 text-left space-y-1">
              <li>Compete on a private leaderboard</li>
              <li>See real-time updates when friends log applications</li>
              <li>Track who's ahead and stay motivated</li>
              <li>Celebrate wins together!</li>
            </ul>
          </div>
        </div>

        <CreatePartyModal
          isOpen={showCreateModal}
          onClose={() => setShowCreateModal(false)}
          onCreated={handlePartyCreated}
        />

        <JoinPartyModal
          isOpen={showJoinModal}
          onClose={() => setShowJoinModal(false)}
          onJoined={handlePartyJoined}
        />
      </div>
    );
  }

  // Has party - show party dashboard
  return (
    <div className="max-w-4xl mx-auto">
      {/* Header */}
      <div className="flex items-center justify-between mb-6">
        <h1 className="font-arcade text-2xl text-amber">{party.name}</h1>
        <div className="flex items-center gap-2">
          {connectionState === HubConnectionState.Connected && (
            <span className="flex items-center gap-1 text-xs text-terminal">
              <span className="w-2 h-2 bg-terminal rounded-full animate-pulse" />
              Live
            </span>
          )}
          {connectionState === HubConnectionState.Reconnecting && (
            <span className="flex items-center gap-1 text-xs text-amber">
              <span className="w-2 h-2 bg-amber rounded-full animate-pulse" />
              Reconnecting...
            </span>
          )}
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Left Column - Leaderboard */}
        <div className="lg:col-span-2 space-y-6">
          <Leaderboard
            entries={leaderboard}
            isLoading={isLeaderboardLoading}
            currentUserId={user?.userId}
          />
          <ActivityFeed
            events={activityEvents}
            isLoading={isActivityLoading}
            connectionState={activityConnectionState}
            hasMore={activityHasMore}
          />
        </div>

        {/* Right Column - Rivalry Panel & Party Card */}
        <div className="space-y-6">
          <RivalryPanel rivalry={rivalry} isLoading={isLeaderboardLoading} />
          <PartyCard party={party} onLeave={handleLeaveParty} />
        </div>
      </div>
    </div>
  );
}
