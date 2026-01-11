import type { ActivityEvent } from '@/types/activity';
import { formatRelativeDate } from '@/utils/date';

interface ActivityEventCardProps {
  event: ActivityEvent;
}

const getInitials = (name: string): string => {
  const parts = name.trim().split(/\s+/).filter(Boolean);
  if (parts.length === 0) return '?';
  if (parts.length === 1) return parts[0].slice(0, 2).toUpperCase();
  return `${parts[0][0]}${parts[parts.length - 1][0]}`.toUpperCase();
};

const getEventMeta = (event: ActivityEvent) => {
  const companyName = event.companyName || 'a company';

  switch (event.eventType) {
    case 'ApplicationLogged':
      return { label: 'logged an application', accent: 'border-amber', icon: '📝' };
    case 'OfferReceived':
      return { label: `received an offer from ${companyName}`, accent: 'border-blaze', icon: '🎉' };
    case 'Screening':
      return { label: `went through screening with ${companyName}`, accent: 'border-terminal', icon: '📞' };
    case 'Interview':
      return { label: `had an interview with ${companyName}`, accent: 'border-terminal', icon: '💼' };
    case 'Rejected':
      return { label: `was rejected from ${companyName}`, accent: 'border-red-500', icon: '❌' };
    case 'Withdrawn':
      return { label: `withdrew from ${companyName}`, accent: 'border-gray-500', icon: '🚫' };
    case 'MilestoneHit':
      return { label: event.milestoneLabel ?? 'hit a milestone', accent: 'border-terminal', icon: '🏁' };
    case 'StatusUpdated':
    default:
      return { label: 'updated a status', accent: 'border-sky-400', icon: '📈' };
  }
};

export default function ActivityEventCard({ event }: ActivityEventCardProps) {
  const meta = getEventMeta(event);
  const initials = getInitials(event.userDisplayName);
  const pointsText =
    event.pointsDelta > 0 ? `+${event.pointsDelta}` : event.pointsDelta.toString();
  const pointsColor = event.pointsDelta >= 0 ? 'text-terminal' : 'text-blaze';

  // For events that mention company in the label, only show role title in detail
  const showCompanyInDetail = event.eventType === 'ApplicationLogged' || event.eventType === 'StatusUpdated' || event.eventType === 'MilestoneHit';
  const detailParts = showCompanyInDetail
    ? [event.roleTitle, event.companyName].filter(Boolean)
    : [event.roleTitle].filter(Boolean);
  const detail = detailParts.join(' · ');

  return (
    <article
      tabIndex={0}
      className={`group relative rounded-lg border-l-4 ${meta.accent} bg-slate-900/60 px-4 py-3 shadow-sm transition hover:bg-slate-900/80 focus:outline-none focus:ring-2 focus:ring-amber/60`}
      aria-label={`${event.userDisplayName} ${meta.label}`}
    >
      <div className="flex items-start justify-between gap-3">
        <div className="flex items-start gap-3">
          <div className="flex h-10 w-10 items-center justify-center rounded-full bg-slate-800 text-sm font-bold text-amber">
            {initials}
          </div>
          <div>
            <div className="flex items-center gap-2 text-sm text-gray-200">
              <span className="text-lg">{meta.icon}</span>
              <span className="font-semibold text-amber">{event.userDisplayName}</span>
              <span className="text-gray-400">{meta.label}</span>
            </div>
            {detail && (
              <div className="text-xs text-gray-400 mt-1">{detail}</div>
            )}
          </div>
        </div>
        <div className="text-right">
          <div className={`text-sm font-bold ${pointsColor}`}>{pointsText} pts</div>
          <div className="text-xs text-gray-500">{formatRelativeDate(event.createdDate)}</div>
        </div>
      </div>
      {event.eventType === 'MilestoneHit' && event.milestoneLabel && (
        <div className="mt-2 text-xs font-semibold text-terminal">
          {event.milestoneLabel}
        </div>
      )}
    </article>
  );
}
