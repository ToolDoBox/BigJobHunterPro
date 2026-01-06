const relativeFormatter = new Intl.RelativeTimeFormat('en', { numeric: 'auto' });

export const formatRelativeDate = (isoDate: string): string => {
  const parsed = new Date(isoDate);

  if (Number.isNaN(parsed.getTime())) {
    return '';
  }

  const now = new Date();
  const diffMs = parsed.getTime() - now.getTime();
  const diffSeconds = Math.round(diffMs / 1000);

  if (Math.abs(diffSeconds) < 30) {
    return 'just now';
  }

  const diffMinutes = Math.round(diffSeconds / 60);
  const diffHours = Math.round(diffMinutes / 60);
  const diffDays = Math.round(diffHours / 24);

  if (Math.abs(diffDays) >= 730) {
    return parsed.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  }

  if (Math.abs(diffDays) >= 1) {
    return relativeFormatter.format(diffDays, 'day');
  }

  if (Math.abs(diffHours) >= 1) {
    return relativeFormatter.format(diffHours, 'hour');
  }

  if (Math.abs(diffMinutes) >= 1) {
    return relativeFormatter.format(diffMinutes, 'minute');
  }

  return relativeFormatter.format(diffSeconds, 'second');
};
