import { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import NavLink from './NavLink';
import { useAuth } from '@/hooks/useAuth';

const navItems = [
  { to: '/app/dashboard', label: 'The Lodge', icon: '🏠' },
  { to: '/app/applications', label: 'The Armory', icon: '🎯' },
  { to: '/app/party', label: 'Hunting Party', icon: '🏆' },
  { to: '/app/profile', label: 'Profile', icon: '👤' },
];

export default function Header() {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const { user, logout, isLoading } = useAuth();
  const location = useLocation();

  const handleLogout = async () => {
    await logout();
  };

  return (
    <header className="header-bar">
      <div className="max-w-7xl mx-auto px-4 sm:px-6">
        <div className="flex justify-between items-center h-16">
          {/* Logo */}
          <Link to="/app" className="flex items-center gap-3 logo-badge">
            <h1 className="font-arcade text-sm md:text-base text-blaze">
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
          <div className="flex items-center gap-4">
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
                onClick={() => setIsMenuOpen(!isMenuOpen)}
                className="btn-metal flex items-center gap-2"
              >
                <span className="text-amber font-semibold">
                  {user?.displayName ?? 'Hunter'}
                </span>
                <span className="text-terminal">{isMenuOpen ? '▲' : '▼'}</span>
              </button>

              {/* Dropdown menu */}
              {isMenuOpen && (
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
              onClick={() => setIsMenuOpen(!isMenuOpen)}
              className="md:hidden p-2 text-amber hover:text-blaze bg-transparent border-none cursor-pointer"
            >
              <span className="text-2xl">{isMenuOpen ? '✕' : '☰'}</span>
            </button>
          </div>
        </div>

        {/* Mobile Navigation */}
        {isMenuOpen && (
          <nav className="md:hidden py-4 border-t border-metal-border">
            {navItems.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                isActive={location.pathname.startsWith(item.to)}
                className="block py-2"
                onClick={() => setIsMenuOpen(false)}
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
