import type { ApplicationListItem } from '@/types/application';
import StatusBadge from './StatusBadge';
import { formatRelativeDate } from '@/utils/date';
import { Link } from 'react-router-dom';

interface ApplicationCardProps {
  application: ApplicationListItem;
}

export default function ApplicationCard({ application }: ApplicationCardProps) {
  const companyDisplay = application.companyName?.trim() || 'Pending company';
  const roleDisplay = application.roleTitle?.trim() || 'Pending role';

  return (
    <Link
      to={`/app/applications/${application.id}`}
      className="rounded-lg border border-metal-border bg-metal p-4 shadow-sm transition-colors hover:border-amber/40 focus-visible:outline focus-visible:outline-2 focus-visible:outline-amber/60"
      aria-label={`View ${companyDisplay} application`}
    >
      <div className="flex items-start justify-between gap-3">
        <div className="min-w-0">
          <div className="text-amber font-semibold truncate" title={companyDisplay}>
            {companyDisplay}
          </div>
          <div className="text-gray-300 text-sm mt-1 truncate" title={roleDisplay}>
            {roleDisplay}
          </div>
        </div>
        <StatusBadge status={application.status} />
      </div>
      <div className="text-gray-400 text-xs mt-3">
        {formatRelativeDate(application.createdDate)}
      </div>
    </Link>
  );
}
