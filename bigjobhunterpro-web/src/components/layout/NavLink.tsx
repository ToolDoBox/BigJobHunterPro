import { Link } from 'react-router-dom';
import type { ReactNode } from 'react';

interface NavLinkProps {
  to: string;
  children: ReactNode;
  isActive?: boolean;
  className?: string;
  onClick?: () => void;
}

export default function NavLink({
  to,
  children,
  isActive = false,
  className = '',
  onClick,
}: NavLinkProps) {
  return (
    <Link
      to={to}
      onClick={onClick}
      className={`
        nav-link font-body text-sm uppercase tracking-wide
        transition-all duration-200
        ${isActive
          ? 'text-blaze font-semibold'
          : 'text-gray-300 hover:text-amber'
        }
        ${className}
      `}
    >
      {children}
    </Link>
  );
}
