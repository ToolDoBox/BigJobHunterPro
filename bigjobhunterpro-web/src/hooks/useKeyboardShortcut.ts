import { useEffect, useCallback } from 'react';

export function useKeyboardShortcut(
  key: string,
  callback: () => void,
  options: { ctrl?: boolean; meta?: boolean; disabled?: boolean } = {}
) {
  const handleKeyDown = useCallback((e: KeyboardEvent) => {
    if (options.disabled) return;

    const ctrlOrMeta = options.ctrl || options.meta;
    const hasModifier = ctrlOrMeta ? (e.ctrlKey || e.metaKey) : true;

    if (e.key.toLowerCase() === key.toLowerCase() && hasModifier) {
      e.preventDefault();
      callback();
    }
  }, [key, callback, options]);

  useEffect(() => {
    document.addEventListener('keydown', handleKeyDown);
    return () => document.removeEventListener('keydown', handleKeyDown);
  }, [handleKeyDown]);
}
