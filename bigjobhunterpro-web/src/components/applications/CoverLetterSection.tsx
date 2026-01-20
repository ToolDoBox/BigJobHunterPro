import { useState } from 'react';
import Modal from '@/components/ui/Modal';
import coverLetterService from '@/services/coverLetter';
import { useToast } from '@/context/ToastContext';
import { formatRelativeDate } from '@/utils/date';

interface CoverLetterSectionProps {
  applicationId: string;
  coverLetterHtml: string | null;
  coverLetterGeneratedAt: string | null;
  onUpdated: () => void;
}

export default function CoverLetterSection({
  applicationId,
  coverLetterHtml,
  coverLetterGeneratedAt,
  onUpdated,
}: CoverLetterSectionProps) {
  const { showToast } = useToast();
  const [isGenerating, setIsGenerating] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [showResumeModal, setShowResumeModal] = useState(false);
  const [resumeInput, setResumeInput] = useState('');
  const [editContent, setEditContent] = useState('');

  const handleGenerate = async (resumeText?: string) => {
    setIsGenerating(true);
    setShowResumeModal(false);

    try {
      const result = await coverLetterService.generate(applicationId, resumeText);

      if (result.success) {
        showToast('success', 'Cover letter generated successfully!');
        onUpdated();
      } else {
        // If the error indicates no resume, show the modal
        if (result.errorMessage?.toLowerCase().includes('resume')) {
          setShowResumeModal(true);
        } else {
          showToast('error', result.errorMessage ?? 'Failed to generate cover letter');
        }
      }
    } catch {
      showToast('error', 'Failed to generate cover letter');
    } finally {
      setIsGenerating(false);
    }
  };

  const handleResumeSubmit = () => {
    if (!resumeInput.trim()) {
      showToast('error', 'Please paste your resume');
      return;
    }
    handleGenerate(resumeInput);
  };

  const handleStartEdit = () => {
    setEditContent(coverLetterHtml || '');
    setIsEditing(true);
  };

  const handleSaveEdit = async () => {
    if (!editContent.trim()) {
      showToast('error', 'Cover letter cannot be empty');
      return;
    }

    setIsSaving(true);
    try {
      const result = await coverLetterService.save(applicationId, editContent);

      if (result.success) {
        showToast('success', 'Cover letter saved successfully!');
        setIsEditing(false);
        onUpdated();
      } else {
        showToast('error', result.errorMessage ?? 'Failed to save cover letter');
      }
    } catch {
      showToast('error', 'Failed to save cover letter');
    } finally {
      setIsSaving(false);
    }
  };

  const handleCancelEdit = () => {
    setIsEditing(false);
    setEditContent('');
  };

  const handleCopy = async () => {
    if (!coverLetterHtml) return;

    try {
      // Extract text content from HTML for plain text copy
      const tempDiv = document.createElement('div');
      tempDiv.innerHTML = coverLetterHtml;
      const textContent = tempDiv.textContent || tempDiv.innerText || '';

      await navigator.clipboard.writeText(textContent);
      showToast('success', 'Cover letter copied to clipboard!');
    } catch {
      showToast('error', 'Failed to copy to clipboard');
    }
  };

  const handleCopyHtml = async () => {
    if (!coverLetterHtml) return;

    try {
      await navigator.clipboard.writeText(coverLetterHtml);
      showToast('success', 'HTML copied to clipboard!');
    } catch {
      showToast('error', 'Failed to copy HTML');
    }
  };

  return (
    <div>
      <h3 className="font-arcade text-xs text-amber mb-3">COVER LETTER</h3>

      {!coverLetterHtml && !isEditing && (
        <div className="text-center py-8 bg-metal-dark/30 rounded-lg border border-metal-light/20">
          <p className="text-gray-400 mb-4">
            No cover letter generated yet. Generate one using AI based on your resume and this job description.
          </p>
          <button
            className="btn-metal-primary"
            onClick={() => handleGenerate()}
            disabled={isGenerating}
          >
            {isGenerating ? 'GENERATING...' : 'GENERATE COVER LETTER'}
          </button>
        </div>
      )}

      {coverLetterHtml && !isEditing && (
        <div>
          <div className="flex flex-wrap gap-2 mb-4">
            <button
              className="btn-metal"
              onClick={handleStartEdit}
              title="Edit cover letter"
            >
              EDIT
            </button>
            <button
              className="btn-metal"
              onClick={() => handleGenerate()}
              disabled={isGenerating}
              title="Regenerate cover letter"
            >
              {isGenerating ? 'REGENERATING...' : 'REGENERATE'}
            </button>
            <button
              className="btn-metal"
              onClick={handleCopy}
              title="Copy as plain text"
            >
              COPY TEXT
            </button>
            <button
              className="btn-metal"
              onClick={handleCopyHtml}
              title="Copy HTML source"
            >
              COPY HTML
            </button>
          </div>

          {coverLetterGeneratedAt && (
            <p className="text-xs text-gray-400 mb-3">
              Generated {formatRelativeDate(coverLetterGeneratedAt)}
            </p>
          )}

          {/* Cover letter preview in iframe for isolated styling */}
          <div className="border border-metal-light/30 rounded-lg overflow-hidden bg-white">
            <iframe
              title="Cover Letter Preview"
              srcDoc={coverLetterHtml}
              className="w-full h-[600px] border-0"
              sandbox="allow-same-origin"
            />
          </div>
        </div>
      )}

      {isEditing && (
        <div>
          <div className="mb-4">
            <label htmlFor="coverLetterEdit" className="block text-xs text-gray-400 mb-2">
              Edit the HTML cover letter below:
            </label>
            <textarea
              id="coverLetterEdit"
              className="input-arcade w-full font-mono text-sm"
              rows={20}
              value={editContent}
              onChange={(e) => setEditContent(e.target.value)}
              disabled={isSaving}
            />
          </div>
          <div className="flex gap-2">
            <button
              className="btn-metal"
              onClick={handleCancelEdit}
              disabled={isSaving}
            >
              CANCEL
            </button>
            <button
              className="btn-metal-primary"
              onClick={handleSaveEdit}
              disabled={isSaving}
            >
              {isSaving ? 'SAVING...' : 'SAVE CHANGES'}
            </button>
          </div>
        </div>
      )}

      {/* Resume Input Modal */}
      <Modal
        isOpen={showResumeModal}
        onClose={() => setShowResumeModal(false)}
        title="PASTE YOUR RESUME"
      >
        <p className="text-gray-300 mb-4">
          No saved resume found. Please paste your resume text below to generate a cover letter.
        </p>
        <textarea
          className="input-arcade w-full mb-4"
          rows={10}
          placeholder="Paste your resume here..."
          value={resumeInput}
          onChange={(e) => setResumeInput(e.target.value)}
        />
        <div className="flex gap-3">
          <button
            type="button"
            className="btn-metal flex-1"
            onClick={() => setShowResumeModal(false)}
          >
            CANCEL
          </button>
          <button
            type="button"
            className="btn-metal-primary flex-1"
            onClick={handleResumeSubmit}
            disabled={!resumeInput.trim()}
          >
            GENERATE
          </button>
        </div>
      </Modal>
    </div>
  );
}
