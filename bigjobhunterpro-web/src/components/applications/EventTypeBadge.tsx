import type { EventType } from '../../types/timelineEvent';

interface EventTypeBadgeProps {
  eventType: EventType;
  interviewRound?: number | null;
}

const EVENT_TYPE_CLASS_MAP: Record<EventType, string> = {
  Prospecting: 'event-badge-prospecting',
  Applied: 'event-badge-applied',
  Screening: 'event-badge-screening',
  Interview: 'event-badge-interview',
  Offer: 'event-badge-offer',
  Rejected: 'event-badge-rejected',
  Withdrawn: 'event-badge-withdrawn',
};

export function EventTypeBadge({ eventType, interviewRound }: EventTypeBadgeProps) {
  const badgeClass = EVENT_TYPE_CLASS_MAP[eventType] || 'event-badge-applied';

  const displayText = eventType === 'Interview' && interviewRound
    ? `Interview - Round ${interviewRound}`
    : eventType;

  return (
    <span className={`event-badge ${badgeClass}`}>
      {displayText}
    </span>
  );
}
