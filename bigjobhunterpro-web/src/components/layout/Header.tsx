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
    <header className="bg-charcoal border-b-4 border-blaze shadow-orange-glow">
      <div className="max-w-7xl mx-auto px-4 sm:px-6">
        <div className="flex justify-between items-center h-16">
          {/* Logo */}
          <Link to="/app" className="flex items-center gap-3">
            <h1 className="font-arcade text-sm md:text-base text-blaze text-glow-blaze">
              BIG JOB HUNTER PRO
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
            {/* Points display */}
            <div className="hidden sm:flex items-center gap-2 bg-forest/50 px-3 py-1 border border-terminal">
              <span className="font-arcade text-xs text-terminal text-glow-terminal">
                {user?.points ?? 0} PTS
              </span>
            </div>

            {/* User dropdown */}
            <div className="relative">
              <button
                onClick={() => setIsMenuOpen(!isMenuOpen)}
                className="flex items-center gap-2 px-3 py-2 border-2 border-forest
                           hover:border-blaze transition-colors bg-transparent"
              >
                <span className="text-amber font-semibold">
                  {user?.displayName ?? 'Hunter'}
                </span>
                <span className="text-terminal">{isMenuOpen ? '▲' : '▼'}</span>
              </button>

              {/* Dropdown menu */}
              {isMenuOpen && (
                <div className="absolute right-0 mt-2 w-48 bg-charcoal border-2 border-blaze
                                shadow-orange-glow z-50">
                  <div className="p-3 border-b border-forest">
                    <p className="text-xs text-gray-400">Signed in as</p>
                    <p className="text-sm text-amber truncate">{user?.email}</p>
                  </div>
                  <button
                    onClick={handleLogout}
                    disabled={isLoading}
                    className="w-full text-left px-3 py-2 text-gray-100
                               hover:bg-forest/50 hover:text-blaze transition-colors
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
          <nav className="md:hidden py-4 border-t border-forest">
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
