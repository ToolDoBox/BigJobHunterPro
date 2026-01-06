import type { ApplicationListItem } from '@/types/application';
import ApplicationRow from './ApplicationRow';
import ApplicationCard from './ApplicationCard';

interface ApplicationsListProps {
  items: ApplicationListItem[];
}

export default function ApplicationsList({ items }: ApplicationsListProps) {
  return (
    <div className="space-y-4">
      <div className="hidden md:block">
        <div className="overflow-hidden rounded-lg border border-metal-border/60">
          <table className="min-w-full text-sm">
            <thead className="bg-metal text-gray-400">
              <tr>
                <th className="text-left py-3 pl-4 pr-4 font-arcade text-xs tracking-wide">
                  COMPANY
                </th>
                <th className="text-left py-3 pr-4 font-arcade text-xs tracking-wide">
                  ROLE
                </th>
                <th className="text-left py-3 pr-4 font-arcade text-xs tracking-wide">
                  STATUS
                </th>
                <th className="text-left py-3 pr-4 font-arcade text-xs tracking-wide">
                  DATE APPLIED
                </th>
              </tr>
            </thead>
            <tbody>
              {items.map((application) => (
                <ApplicationRow key={application.id} application={application} />
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <div className="grid gap-4 md:hidden">
        {items.map((application) => (
          <ApplicationCard key={application.id} application={application} />
        ))}
      </div>
    </div>
  );
}
