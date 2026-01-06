interface StatusBadgeProps {
  status: string;
}

const statusClassMap: Record<string, string> = {
  applied: 'status-badge status-badge-applied',
  screening: 'status-badge status-badge-screening',
  interview: 'status-badge status-badge-interview',
  offer: 'status-badge status-badge-offer',
  rejected: 'status-badge status-badge-rejected',
  withdrawn: 'status-badge status-badge-withdrawn',
};

export default function StatusBadge({ status }: StatusBadgeProps) {
  const key = status.trim().toLowerCase();
  const className = statusClassMap[key] ?? 'status-badge status-badge-unknown';
  const label = status ? status.toUpperCase() : 'UNKNOWN';

  return (
    <span className={className}>{label}</span>
  );
}
