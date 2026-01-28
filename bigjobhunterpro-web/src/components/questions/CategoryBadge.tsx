import type { QuestionCategory } from '@/types/question';
import { getCategoryLabel, getCategoryColor } from '@/types/question';

interface CategoryBadgeProps {
  category: QuestionCategory;
  className?: string;
}

export default function CategoryBadge({ category, className = '' }: CategoryBadgeProps) {
  const colorClasses = getCategoryColor(category);
  const label = getCategoryLabel(category);

  return (
    <span
      className={`inline-flex items-center px-2 py-0.5 rounded text-xs font-medium border ${colorClasses} ${className}`}
    >
      {label}
    </span>
  );
}
