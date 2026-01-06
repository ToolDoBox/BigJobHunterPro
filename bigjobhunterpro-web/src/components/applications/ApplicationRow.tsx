import type { KeyboardEvent } from 'react';
import type { ApplicationListItem } from '@/types/application';
import StatusBadge from './StatusBadge';
import { formatRelativeDate } from '@/utils/date';
import { useNavigate } from 'react-router-dom';

interface ApplicationRowProps {
  application: ApplicationListItem;
}

export default function ApplicationRow({ application }: ApplicationRowProps) {
  const navigate = useNavigate();
  const companyDisplay = application.companyName?.trim() || 'Pending company';
  const roleDisplay = application.roleTitle?.trim() || 'Pending role';

  const handleSelect = () => {
    navigate(`/app/applications/${application.id}`);
  };

  const handleKeyDown = (event: KeyboardEvent<HTMLTableRowElement>) => {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      handleSelect();
    }
  };

  return (
    <tr
      className="border-b border-metal-border/60 hover:bg-metal-light/30 transition-colors cursor-pointer"
      onClick={handleSelect}
      onKeyDown={handleKeyDown}
      role="link"
      tabIndex={0}
    >
      <td className="py-3 pl-4 pr-4">
        <div
          className="font-semibold text-amber truncate max-w-[240px]"
          title={companyDisplay}
        >
          {companyDisplay}
        </div>
      </td>
      <td className="py-3 pr-4">
        <div className="text-gray-300 truncate max-w-[240px]" title={roleDisplay}>
          {roleDisplay}
        </div>
      </td>
      <td className="py-3 pr-4">
        <StatusBadge status={application.status} />
      </td>
      <td className="py-3 pr-4 text-gray-400 text-sm">
        {formatRelativeDate(application.createdDate)}
      </td>
    </tr>
  );
}
