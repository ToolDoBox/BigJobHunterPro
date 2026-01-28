import { useEffect, useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import NavLink from './NavLink';
import { useAuth } from '@/hooks/useAuth';

const navItems = [
  { to: '/app/dashboard', label: 'The Lodge', icon: '🏠' },
  { to: '/app/applications', label: 'The Armory', icon: '🎯' },
  { to: '/app/question-range', label: 'Question Range', icon: '📋' },
  { to: '/app/party', label: 'Hunting Party', icon: '🏆' },
  { to: '/app/profile', label: 'Profile', icon: '👤' },
];

export default function Header() {
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const { user, logout, isLoading } = useAuth();
  const location = useLocation();

  const handleLogout = async () => {
    await logout();
    setIsUserMenuOpen(false);
  };

  useEffect(() => {
    setIsUserMenuOpen(false);
    setIsMobileMenuOpen(false);
  }, [location.pathname]);

  return (
    <header className="header-bar">
      <div className="max-w-7xl mx-auto px-2 sm:px-4 md:px-6">
        <div className="flex justify-between items-center h-16 gap-2">
          {/* Logo */}
          <Link to="/app" className="flex items-center logo-badge flex-shrink-0">
            <h1 className="font-arcade text-[10px] xs:text-xs sm:text-sm md:text-base text-blaze whitespace-nowrap">
              JOB HUNTER PRO
            </h1>
          </Link>

          {/* Desktop Navigation */}
          <nav className="hidden md:flex items-center gap-6">
            {navItems.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                isActive={location.pathname.startsWith(item.to)}
              >
                <span className="mr-2">{item.icon}</span>
                {item.label}
              </NavLink>
            ))}
          </nav>

          {/* User menu */}
          <div className="flex items-center gap-1 sm:gap-2 md:gap-4">
            {/* Points display - stat display style */}
            <div className="hidden sm:flex stat-display">
              <span className="stat-display-value">
                {user?.points ?? 0}
              </span>
              <span className="stat-display-label ml-2">PTS</span>
            </div>

            {/* User dropdown */}
            <div className="relative">
              <button
                onClick={() => {
                  setIsUserMenuOpen((open) => !open);
                  setIsMobileMenuOpen(false);
                }}
                className="btn-metal flex items-center gap-1 sm:gap-2 text-xs sm:text-sm px-2 sm:px-3"
              >
                <span className="text-amber font-semibold truncate max-w-[80px] sm:max-w-none">
                  {user?.displayName ?? 'Hunter'}
                </span>
                <span className="text-terminal flex-shrink-0">{isUserMenuOpen ? '▲' : '▼'}</span>
              </button>

              {/* Dropdown menu */}
              {isUserMenuOpen && (
                <div className="absolute right-0 mt-2 w-48 metal-panel z-50">
                  <div className="p-3 border-b border-metal-border">
                    <p className="text-xs text-gray-500">Signed in as</p>
                    <p className="text-sm text-terminal truncate">{user?.email}</p>
                  </div>
                  <button
                    onClick={handleLogout}
                    disabled={isLoading}
                    className="w-full text-left px-3 py-2 text-gray-300
                               hover:bg-metal-light hover:text-blaze transition-colors
                               disabled:opacity-50 bg-transparent border-none cursor-pointer"
                  >
                    {isLoading ? 'Logging out...' : 'Logout'}
                  </button>
                </div>
              )}
            </div>

            {/* Mobile menu button */}
            <button
              onClick={() => { setIsMobileMenuOpen((open) => !open); setIsUserMenuOpen(false); }}
              className="md:hidden p-2 text-amber hover:text-blaze bg-transparent border-none cursor-pointer"
            >
              <span className="text-2xl">{isMobileMenuOpen ? '✕' : '☰'}</span>
            </button>
          </div>
        </div>

        {/* Mobile Navigation */}
        {isMobileMenuOpen && (
          <nav className="md:hidden py-4 border-t border-metal-border">
            {navItems.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                isActive={location.pathname.startsWith(item.to)}
                className="block py-2"
                onClick={() => setIsMobileMenuOpen(false)}
              >
                <span className="mr-2">{item.icon}</span>
                {item.label}
              </NavLink>
            ))}
          </nav>
        )}
      </div>
    </header>
  );
}
