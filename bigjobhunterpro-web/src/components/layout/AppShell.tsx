import { Outlet } from 'react-router-dom';
import Header from './Header';
import ToastContainer from '@/components/ui/ToastContainer';

export default function AppShell() {
  return (
    <div className="min-h-screen bg-metal-dark app-shell flex flex-col">
      {/* Header with navigation */}
      <Header />

      {/* Main content area */}
      <main className="flex-1 p-6 max-w-7xl mx-auto w-full hud-frame">
        <Outlet />
      </main>

      {/* Footer - Industrial style */}
      <footer className="border-t-2 border-metal-border hud-footer py-4">
        <div className="text-center space-y-1">
          <div className="pixel-text-sm text-terminal">
            Big Job Hunter Pro v0.1
          </div>
          <div className="text-xs text-gray-400">
            Crafted in co-op mode by{' '}
            <a
              href="https://christianadleta.com"
              target="_blank"
              rel="noopener noreferrer"
              className="text-amber hover:text-blaze transition-colors"
            >
              Christian Adleta
            </a>
            {' & '}
            <a
              href="https://emmettshaughnessy.com"
              target="_blank"
              rel="noopener noreferrer"
              className="text-amber hover:text-blaze transition-colors"
            >
              Emmett Shaughnessy
            </a>
          </div>
        </div>
      </footer>

      {/* Toast notifications */}
      <ToastContainer />
    </div>
  );
}
