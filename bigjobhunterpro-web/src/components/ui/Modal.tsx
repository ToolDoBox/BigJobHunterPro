import { useEffect, useRef, useState, type ReactNode } from 'react';

interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode;
  showCloseButton?: boolean;
}

export default function Modal({
  isOpen,
  onClose,
  title,
  children,
  showCloseButton = true,
}: ModalProps) {
  const [isVisible, setIsVisible] = useState(isOpen);
  const [isClosing, setIsClosing] = useState(false);
  const modalRef = useRef<HTMLDivElement>(null);
  const closeTimer = useRef<number | null>(null);
  const ANIMATION_DURATION_MS = 200;

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
      modalRef.current?.focus();
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
      return;
    }

    if (isVisible) {
      setIsClosing(true);
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
  }, []);

  if (!isVisible) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-start justify-center overflow-y-auto px-4 py-6 sm:items-center sm:py-8">
      {/* Backdrop */}
      <div
        className={`absolute inset-0 bg-black/70 backdrop-blur-sm ${
          isClosing ? 'animate-fade-out' : 'animate-fade-in'
        } motion-reduce:animate-none`}
        onClick={onClose}
      />

      {/* Modal */}
      <div
        ref={modalRef}
        role="dialog"
        aria-modal="true"
        aria-labelledby="modal-title"
        tabIndex={-1}
        className={`relative metal-panel metal-panel-orange w-full max-w-md max-h-[90vh] overflow-y-auto ${
          isClosing ? 'animate-scale-out' : 'animate-scale-in'
        } motion-reduce:animate-none`}
      >
        <div className="metal-panel-screws" />

        {/* Header */}
        <div className="flex items-center justify-between mb-6">
          <h2 id="modal-title" className="font-arcade text-lg text-blaze">
            {title}
          </h2>
          {showCloseButton && (
            <button
              onClick={onClose}
              className="text-gray-400 hover:text-white transition-colors"
              aria-label="Close modal"
            >
              <span className="font-arcade text-xs">X</span>
            </button>
          )}
        </div>

        {children}
      </div>
    </div>
  );
}
