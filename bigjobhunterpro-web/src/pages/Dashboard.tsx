import { useState, useEffect } from 'react';
import { useAuth } from '@/hooks/useAuth';
import { useKeyboardShortcut } from '@/hooks/useKeyboardShortcut';
import QuickCaptureModal from '@/components/applications/QuickCaptureModal';
import { getWeeklyStats } from '@/services/statistics';
import type { WeeklyStats } from '@/services/statistics';
import { getTopKeywords, getConversionBySource } from '@/services/analytics';
import type { KeywordFrequency, ConversionBySource } from '@/services/analytics';

export default function Dashboard() {
  const { user } = useAuth();
  const [isQuickCaptureOpen, setIsQuickCaptureOpen] = useState(false);
  const [weeklyStats, setWeeklyStats] = useState<WeeklyStats | null>(null);
  const [isLoadingStats, setIsLoadingStats] = useState(false);
  const [keywords, setKeywords] = useState<KeywordFrequency[]>([]);
  const [conversions, setConversions] = useState<ConversionBySource[]>([]);
  const [isLoadingAnalytics, setIsLoadingAnalytics] = useState(false);

  // Ctrl+K / Cmd+K shortcut to open Quick Capture
  useKeyboardShortcut('k', () => setIsQuickCaptureOpen(true), {
    ctrl: true,
    disabled: isQuickCaptureOpen, // Don't trigger when already open
  });

  // Fetch weekly statistics
  useEffect(() => {
    const fetchWeeklyStats = async () => {
      if (!user) return;

      setIsLoadingStats(true);
      try {
        const stats = await getWeeklyStats();
        setWeeklyStats(stats);
      } catch (error) {
        console.error('Failed to fetch weekly statistics:', error);
        // Fail silently - stats are nice-to-have
      } finally {
        setIsLoadingStats(false);
      }
    };

    fetchWeeklyStats();
  }, [user]);

  // Fetch analytics data
  useEffect(() => {
    const fetchAnalytics = async () => {
      if (!user) return;

      setIsLoadingAnalytics(true);
      try {
        const [keywordsData, conversionsData] = await Promise.all([
          getTopKeywords(10), // Get top 10 keywords
          getConversionBySource()
        ]);
        setKeywords(keywordsData);
        setConversions(conversionsData);
      } catch (error) {
        console.error('Failed to fetch analytics:', error);
        // Fail silently - analytics are nice-to-have
      } finally {
        setIsLoadingAnalytics(false);
      }
    };

    fetchAnalytics();
  }, [user]);

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
            <span className="stat-display-value text-3xl text-amber">
              {user?.applicationCount ?? 0}
            </span>
          </div>
          <div className="font-arcade text-xs text-gray-500 tracking-wider">
            APPLICATIONS
          </div>
        </div>

        {/* Day Streak */}
        <div className="metal-panel text-center">
          <div className="metal-panel-screws" />
          <div className="stat-display inline-block mb-3">
            <span className="stat-display-value text-3xl text-blaze">
              {user?.currentStreak ?? 0}
            </span>
          </div>
          <div className="font-arcade text-xs text-gray-500 tracking-wider">
            DAY STREAK
          </div>
        </div>
      </div>

      {/* Weekly Progress */}
      {!isLoadingStats && weeklyStats && (
        <div className="metal-panel">
          <div className="metal-panel-screws" />
          <h2 className="font-arcade text-base text-amber mb-4">WEEKLY PROGRESS</h2>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {/* Applications Comparison */}
            <div>
              <div className="text-sm text-gray-400 mb-2">Applications</div>
              <div className="flex items-baseline gap-3">
                <span className="text-2xl font-bold text-terminal-green">
                  {weeklyStats.applicationsThisWeek}
                </span>
                <span className="text-sm text-gray-500">this week</span>
                <span className="text-sm text-gray-600">vs</span>
                <span className="text-lg text-gray-400">
                  {weeklyStats.applicationsLastWeek}
                </span>
                <span className="text-sm text-gray-500">last week</span>
              </div>
              <div className="mt-2">
                {weeklyStats.percentageChange > 0 ? (
                  <span className="text-terminal-green text-sm font-semibold">
                    ↗ +{weeklyStats.percentageChange.toFixed(1)}% from last week
                  </span>
                ) : weeklyStats.percentageChange < 0 ? (
                  <span className="text-red-400 text-sm font-semibold">
                    ↘ {weeklyStats.percentageChange.toFixed(1)}% from last week
                  </span>
                ) : (
                  <span className="text-gray-500 text-sm">
                    → No change from last week
                  </span>
                )}
              </div>
            </div>

            {/* Points Comparison */}
            <div>
              <div className="text-sm text-gray-400 mb-2">Points Earned</div>
              <div className="flex items-baseline gap-3">
                <span className="text-2xl font-bold text-amber">
                  {weeklyStats.pointsThisWeek}
                </span>
                <span className="text-sm text-gray-500">this week</span>
                <span className="text-sm text-gray-600">vs</span>
                <span className="text-lg text-gray-400">
                  {weeklyStats.pointsLastWeek}
                </span>
                <span className="text-sm text-gray-500">last week</span>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* What's Working - Analytics */}
      {!isLoadingAnalytics && (keywords.length > 0 || conversions.length > 0) && (
        <div className="metal-panel">
          <div className="metal-panel-screws" />
          <h2 className="font-arcade text-base text-amber mb-4">WHAT'S WORKING</h2>

          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
            {/* Top Keywords */}
            {keywords.length > 0 && (
              <div>
                <h3 className="text-sm font-semibold text-terminal-green mb-3">
                  Top Keywords in Successful Applications
                </h3>
                <div className="space-y-2">
                  {keywords.slice(0, 10).map((kw, index) => (
                    <div key={kw.keyword} className="flex items-center gap-3">
                      <span className="text-xs text-gray-500 w-5">{index + 1}.</span>
                      <div className="flex-1 flex items-center gap-2">
                        <span className="text-sm font-mono text-gray-200">{kw.keyword}</span>
                        <div className="flex-1 h-2 bg-gray-800 rounded-full overflow-hidden">
                          <div
                            className="h-full bg-terminal-green"
                            style={{ width: `${Math.min(kw.percentage, 100)}%` }}
                          />
                        </div>
                        <span className="text-xs text-gray-400 w-12 text-right">
                          {kw.percentage.toFixed(0)}%
                        </span>
                      </div>
                    </div>
                  ))}
                </div>
                <p className="text-xs text-gray-500 mt-3">
                  * Based on applications that reached interview stage or beyond
                </p>
              </div>
            )}

            {/* Conversion by Source */}
            {conversions.length > 0 && (
              <div>
                <h3 className="text-sm font-semibold text-terminal-green mb-3">
                  Best Performing Sources
                </h3>
                <div className="space-y-2">
                  {conversions.slice(0, 5).map((conv) => (
                    <div key={conv.sourceName} className="flex items-center justify-between p-2 bg-gray-800/50 rounded">
                      <div className="flex-1">
                        <div className="text-sm font-semibold text-gray-200">
                          {conv.sourceName}
                        </div>
                        <div className="text-xs text-gray-500">
                          {conv.interviewCount} interviews from {conv.totalApplications} applications
                        </div>
                      </div>
                      <div className="text-right">
                        <div className={`text-lg font-bold ${
                          conv.conversionRate >= 20 ? 'text-terminal-green' :
                          conv.conversionRate >= 10 ? 'text-amber' :
                          'text-gray-400'
                        }`}>
                          {conv.conversionRate.toFixed(1)}%
                        </div>
                        <div className="text-xs text-gray-500">success rate</div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>

          {keywords.length === 0 && conversions.length === 0 && (
            <p className="text-gray-500 text-center py-8">
              Keep applying! Analytics will appear once you have successful applications.
            </p>
          )}
        </div>
      )}

      {/* Quick actions - Metal panel */}
      <div className="metal-panel">
        <div className="metal-panel-screws" />
        <h2 className="font-arcade text-base text-amber mb-4">QUICK ACTIONS</h2>
        <button
          className="btn-metal-primary"
          onClick={() => setIsQuickCaptureOpen(true)}
        >
          + QUICK CAPTURE
        </button>
      </div>

      {/* Quick Capture Modal */}
      <QuickCaptureModal
        isOpen={isQuickCaptureOpen}
        onClose={() => setIsQuickCaptureOpen(false)}
        onSuccess={() => {
          // Optionally refresh applications list (Story 3)
        }}
      />
    </div>
  );
}
