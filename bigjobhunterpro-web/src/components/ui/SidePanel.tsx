import { useEffect, useRef, useState, type ReactNode } from 'react';

interface SidePanelProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode;
  showCloseButton?: boolean;
}

export default function SidePanel({
  isOpen,
  onClose,
  title,
  children,
  showCloseButton = true,
}: SidePanelProps) {
  const [isVisible, setIsVisible] = useState(isOpen);
  const [isClosing, setIsClosing] = useState(false);
  const panelRef = useRef<HTMLDivElement>(null);
  const closeTimer = useRef<number | null>(null);
  const ANIMATION_DURATION_MS = 300;

  // Handle Escape key
  useEffect(() => {
    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === 'Escape' && isOpen) {
        onClose();
      }
    };
    document.addEventListener('keydown', handleEscape);
    return () => document.removeEventListener('keydown', handleEscape);
  }, [isOpen, onClose]);

  // Focus trap
  useEffect(() => {
    if (isOpen) {
      panelRef.current?.focus();
    }
  }, [isOpen]);

  useEffect(() => {
    if (isOpen) {
      if (closeTimer.current) {
        window.clearTimeout(closeTimer.current);
        closeTimer.current = null;
      }
      setIsVisible(true);
      setIsClosing(false);
      // Add class to body to trigger layout shift
      document.body.classList.add('side-panel-open');
      return;
    }

    if (isVisible) {
      setIsClosing(true);
      // Remove class immediately when starting to close
      document.body.classList.remove('side-panel-open');
      closeTimer.current = window.setTimeout(() => {
        setIsVisible(false);
        setIsClosing(false);
        closeTimer.current = null;
      }, ANIMATION_DURATION_MS);
    }
  }, [isOpen, isVisible]);

  useEffect(() => () => {
    if (closeTimer.current) {
      window.clearTimeout(closeTimer.current);
    }
    // Cleanup: remove class if component unmounts while panel is open
    document.body.classList.remove('side-panel-open');
  }, []);

  if (!isVisible) return null;

  return (
    <>
      {/* Side Panel - No blocking backdrop */}
      <div
        ref={panelRef}
        role="dialog"
        aria-modal="false"
        aria-labelledby="sidepanel-title"
        tabIndex={-1}
        className={`fixed top-0 right-0 z-50 side-panel metal-panel metal-panel-orange h-full overflow-y-auto shadow-2xl ${
          isClosing ? 'animate-slide-out-right' : 'animate-slide-in-right'
        } motion-reduce:animate-none`}
      >
        <div className="metal-panel-screws" />

        {/* Header */}
        <div className="flex items-center justify-between mb-6 sticky top-0 bg-metal z-10 py-4">
          <h2 id="sidepanel-title" className="font-arcade text-lg text-blaze">
            {title}
          </h2>
          {showCloseButton && (
            <button
              onClick={onClose}
              className="text-gray-400 hover:text-white transition-colors"
              aria-label="Close panel"
            >
              <span className="font-arcade text-xs">X</span>
            </button>
          )}
        </div>

        {/* Content */}
        <div className="pb-6">
          {children}
        </div>
      </div>
    </>
  );
}
