import { Outlet } from 'react-router-dom';
import Header from './Header';

export default function AppShell() {
  return (
    <div className="min-h-screen bg-gradient-arcade flex flex-col">
      {/* CRT Scanline overlay */}
      <div className="scanlines" />

      {/* Header with navigation */}
      <Header />

      {/* Main content area */}
      <main className="flex-1 p-6 max-w-7xl mx-auto w-full">
        <Outlet />
      </main>

      {/* Footer */}
      <footer className="border-t-2 border-forest bg-dark-charcoal py-4">
        <div className="text-center">
          <span className="pixel-text-sm text-terminal text-glow-terminal">
            Big Job Hunter Pro v0.1
          </span>
        </div>
      </footer>
    </div>
  );
}
