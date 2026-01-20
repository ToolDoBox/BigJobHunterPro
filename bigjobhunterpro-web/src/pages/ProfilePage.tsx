import { useState, useEffect, useCallback } from 'react';
import { useAuth } from '@/hooks/useAuth';
import { profileService, type ProfileResponse } from '@/services/profile';

const MAX_CHARACTERS = 50000;

export default function ProfilePage() {
  const { user } = useAuth();
  const [profile, setProfile] = useState<ProfileResponse | null>(null);
  const [resumeText, setResumeText] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [hasUnsavedChanges, setHasUnsavedChanges] = useState(false);
  const [showClearConfirm, setShowClearConfirm] = useState(false);

  // Fetch profile on mount
  useEffect(() => {
    const fetchProfile = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const data = await profileService.getProfile();
        setProfile(data);
        setResumeText(data.resumeText ?? '');
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load profile');
      } finally {
        setIsLoading(false);
      }
    };

    fetchProfile();
  }, []);

  // Track unsaved changes
  useEffect(() => {
    if (profile) {
      const originalText = profile.resumeText ?? '';
      setHasUnsavedChanges(resumeText !== originalText);
    }
  }, [resumeText, profile]);

  // Clear success message after 5 seconds
  useEffect(() => {
    if (successMessage) {
      const timer = setTimeout(() => setSuccessMessage(null), 5000);
      return () => clearTimeout(timer);
    }
  }, [successMessage]);

  const handleSave = useCallback(async () => {
    if (!hasUnsavedChanges || isSaving) return;

    setIsSaving(true);
    setError(null);
    setSuccessMessage(null);

    try {
      const response = await profileService.updateResume(resumeText || null);
      setProfile(prev => prev ? {
        ...prev,
        resumeText: resumeText || null,
        resumeUpdatedAt: response.resumeUpdatedAt,
        characterCount: response.characterCount
      } : null);
      setHasUnsavedChanges(false);
      setSuccessMessage(response.message);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to save resume');
    } finally {
      setIsSaving(false);
    }
  }, [resumeText, hasUnsavedChanges, isSaving]);

  const handleClear = useCallback(async () => {
    if (isSaving) return;

    setIsSaving(true);
    setError(null);
    setSuccessMessage(null);
    setShowClearConfirm(false);

    try {
      const response = await profileService.clearResume();
      setResumeText('');
      setProfile(prev => prev ? {
        ...prev,
        resumeText: null,
        resumeUpdatedAt: null,
        characterCount: 0
      } : null);
      setHasUnsavedChanges(false);
      setSuccessMessage(response.message);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to clear resume');
    } finally {
      setIsSaving(false);
    }
  }, [isSaving]);

  const getCharacterCountColor = (count: number): string => {
    if (count > 45000) return 'text-red-400';
    if (count > 15000) return 'text-amber';
    return 'text-terminal-green';
  };

  const getCharacterCountWarning = (count: number): string | null => {
    if (count > 45000) return 'Near limit!';
    if (count > 15000) return 'Getting long!';
    return null;
  };

  const formatDate = (dateString: string | null): string => {
    if (!dateString) return 'Never';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  if (isLoading) {
    return (
      <div className="metal-panel text-center py-12">
        <div className="metal-panel-screws" />
        <div className="text-amber font-arcade animate-pulse">LOADING PROFILE...</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="metal-panel metal-panel-orange">
        <div className="metal-panel-screws" />
        <h1 className="font-arcade text-xl md:text-2xl text-blaze mb-2">
          HUNTER PROFILE
        </h1>
        <p className="text-amber font-semibold">
          {user?.displayName ?? profile?.displayName ?? 'Hunter'}
        </p>
        <p className="text-gray-400 text-sm mt-1">
          {profile?.email}
        </p>
      </div>

      {/* Error Message */}
      {error && (
        <div className="bg-red-900/30 border border-red-500 rounded-lg p-4 text-red-400">
          {error}
        </div>
      )}

      {/* Success Message */}
      {successMessage && (
        <div className="bg-green-900/30 border border-terminal-green rounded-lg p-4 text-terminal-green">
          {successMessage}
        </div>
      )}

      {/* Instructions Panel */}
      <div className="metal-panel">
        <div className="metal-panel-screws" />
        <h2 className="font-arcade text-base text-amber mb-4">HOW TO LOAD YOUR RESUME</h2>
        <div className="space-y-3 text-gray-300">
          <div className="flex items-start gap-3">
            <span className="text-terminal-green font-bold min-w-[24px]">1.</span>
            <span>Open your resume PDF in any PDF viewer</span>
          </div>
          <div className="flex items-start gap-3">
            <span className="text-terminal-green font-bold min-w-[24px]">2.</span>
            <span>Press <kbd className="bg-gray-700 px-2 py-0.5 rounded text-amber font-mono text-sm">Ctrl+A</kbd> to select all text</span>
          </div>
          <div className="flex items-start gap-3">
            <span className="text-terminal-green font-bold min-w-[24px]">3.</span>
            <span>Press <kbd className="bg-gray-700 px-2 py-0.5 rounded text-amber font-mono text-sm">Ctrl+C</kbd> to copy</span>
          </div>
          <div className="flex items-start gap-3">
            <span className="text-terminal-green font-bold min-w-[24px]">4.</span>
            <span>Click in the box below and press <kbd className="bg-gray-700 px-2 py-0.5 rounded text-amber font-mono text-sm">Ctrl+V</kbd> to paste</span>
          </div>
        </div>
        <p className="text-gray-500 text-sm mt-4 italic">
          Tip: Word docs work too! Just Ctrl+A, Ctrl+C from Word, then paste here.
        </p>
      </div>

      {/* Resume Input Panel */}
      <div className="metal-panel">
        <div className="metal-panel-screws" />
        <div className="flex items-center justify-between mb-4">
          <h2 className="font-arcade text-base text-amber">YOUR RESUME</h2>
          {profile?.resumeUpdatedAt && (
            <span className="text-gray-500 text-sm">
              Last saved: {formatDate(profile.resumeUpdatedAt)}
            </span>
          )}
        </div>

        {/* Textarea */}
        <textarea
          value={resumeText}
          onChange={(e) => setResumeText(e.target.value)}
          disabled={isSaving}
          placeholder="Paste your resume text here..."
          className="w-full h-[400px] bg-gray-900 border border-gray-700 rounded-lg p-4 text-gray-200 font-mono text-sm resize-y focus:outline-none focus:border-amber disabled:opacity-50 disabled:cursor-not-allowed"
          maxLength={MAX_CHARACTERS}
        />

        {/* Character Counter */}
        <div className="flex items-center justify-between mt-3">
          <div className="flex items-center gap-3">
            <span className={`font-mono text-sm ${getCharacterCountColor(resumeText.length)}`}>
              {resumeText.length.toLocaleString()} / {MAX_CHARACTERS.toLocaleString()}
            </span>
            {getCharacterCountWarning(resumeText.length) && (
              <span className={`text-sm font-semibold ${getCharacterCountColor(resumeText.length)}`}>
                {getCharacterCountWarning(resumeText.length)}
              </span>
            )}
          </div>

          {/* Unsaved Changes Indicator */}
          {hasUnsavedChanges && (
            <span className="text-amber text-sm animate-pulse">
              You have unsaved changes
            </span>
          )}
        </div>

        {/* Action Buttons */}
        <div className="flex items-center gap-4 mt-6">
          <button
            onClick={handleSave}
            disabled={!hasUnsavedChanges || isSaving}
            className="btn-metal-primary disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {isSaving ? 'SAVING...' : 'SAVE RESUME'}
          </button>

          {resumeText && !showClearConfirm && (
            <button
              onClick={() => setShowClearConfirm(true)}
              disabled={isSaving}
              className="btn-metal-secondary disabled:opacity-50 disabled:cursor-not-allowed"
            >
              CLEAR RESUME
            </button>
          )}

          {showClearConfirm && (
            <div className="flex items-center gap-3">
              <span className="text-gray-400 text-sm">Clear all resume text?</span>
              <button
                onClick={handleClear}
                disabled={isSaving}
                className="px-4 py-2 bg-red-600 hover:bg-red-500 text-white rounded font-semibold text-sm disabled:opacity-50"
              >
                YES, CLEAR
              </button>
              <button
                onClick={() => setShowClearConfirm(false)}
                disabled={isSaving}
                className="px-4 py-2 bg-gray-700 hover:bg-gray-600 text-gray-200 rounded font-semibold text-sm disabled:opacity-50"
              >
                CANCEL
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
