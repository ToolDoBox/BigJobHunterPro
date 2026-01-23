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
      className="block rounded-lg border border-metal-border bg-metal p-4 shadow-sm transition-colors hover:border-amber/40 focus-visible:outline focus-visible:outline-2 focus-visible:outline-amber/60 active:bg-metal-light"
      aria-label={`View ${companyDisplay} application`}
    >
      <div className="flex items-start justify-between gap-3">
        <div className="min-w-0 flex-1">
          <div className="text-amber font-semibold truncate text-base" title={companyDisplay}>
            {companyDisplay}
          </div>
          <div className="text-gray-300 text-sm mt-1.5 truncate" title={roleDisplay}>
            {roleDisplay}
          </div>
        </div>
        <div className="shrink-0">
          <StatusBadge status={application.status} />
        </div>
      </div>
      <div className="text-gray-400 text-xs mt-3">
        {formatRelativeDate(application.createdDate)}
      </div>
    </Link>
  );
}
