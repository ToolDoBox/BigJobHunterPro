import { useRef, useState } from 'react';
import html2canvas from 'html2canvas';
import { jsPDF } from 'jspdf';
import Modal from '@/components/ui/Modal';
import coverLetterService from '@/services/coverLetter';
import { useToast } from '@/context/ToastContext';
import { formatRelativeDate } from '@/utils/date';

interface CoverLetterSectionProps {
  applicationId: string;
  applicantName: string | null;
  companyName: string | null;
  coverLetterHtml: string | null;
  coverLetterGeneratedAt: string | null;
  onUpdated: () => void;
}

export default function CoverLetterSection({
  applicationId,
  applicantName,
  companyName,
  coverLetterHtml,
  coverLetterGeneratedAt,
  onUpdated,
}: CoverLetterSectionProps) {
  const { showToast } = useToast();
  const previewRef = useRef<HTMLIFrameElement | null>(null);
  const [isGenerating, setIsGenerating] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [isExportingPdf, setIsExportingPdf] = useState(false);
  const [isDownloadingCombinedPdf, setIsDownloadingCombinedPdf] = useState(false);
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

  const sanitizeFilenamePart = (value: string) =>
    value
      .replace(/[^a-z0-9]+/gi, '_')
      .replace(/^_+|_+$/g, '');

  const buildPdfFilename = () => {
    const applicantPart = sanitizeFilenamePart(applicantName?.trim() || 'Applicant') || 'Applicant';
    const companyPart = sanitizeFilenamePart(companyName?.trim() || 'Company') || 'Company';
    return `${applicantPart}_${companyPart}_Cover_Letter.pdf`;
  };

  const waitForPreviewReady = (iframe: HTMLIFrameElement) =>
    new Promise<void>((resolve) => {
      if (iframe.contentDocument?.readyState === 'complete') {
        resolve();
        return;
      }

      const handleLoad = () => {
        iframe.removeEventListener('load', handleLoad);
        resolve();
      };

      iframe.addEventListener('load', handleLoad);
    });

  const handleSavePdf = async () => {
    if (!coverLetterHtml) return;

    setIsExportingPdf(true);

    try {
      const iframe = previewRef.current;
      if (!iframe) {
        showToast('error', 'Cover letter preview is not ready yet.');
        return;
      }

      await waitForPreviewReady(iframe);

      const iframeDocument = iframe.contentDocument;
      const sourceElement = iframeDocument?.querySelector('.page') || iframeDocument?.body;

      if (!sourceElement) {
        showToast('error', 'Cover letter preview is not ready yet.');
        return;
      }

      const renderTarget = sourceElement as HTMLElement;
      const computedStyle = iframeDocument?.defaultView?.getComputedStyle(renderTarget);
      const fontSizePx = Number.parseFloat(computedStyle?.fontSize || '16');
      const parsedLineHeight = Number.parseFloat(computedStyle?.lineHeight || '');
      const lineHeightPx = Math.max(
        1,
        Math.round(Number.isFinite(parsedLineHeight) ? parsedLineHeight : fontSizePx * 1.2)
      );
      const spacer = iframeDocument?.createElement('div') ?? null;
      if (spacer) {
        spacer.style.height = `${lineHeightPx}px`;
        spacer.style.width = '100%';
        renderTarget.appendChild(spacer);
      }

      let canvas: HTMLCanvasElement;
      try {
        canvas = await html2canvas(renderTarget, {
          scale: 2,
          useCORS: true,
          backgroundColor: '#ffffff',
          windowWidth: renderTarget.scrollWidth,
          windowHeight: renderTarget.scrollHeight,
        });
      } finally {
        if (spacer && spacer.parentElement) {
          spacer.parentElement.removeChild(spacer);
        }
      }

      const pdf = new jsPDF({
        orientation: 'portrait',
        unit: 'pt',
        format: 'letter',
      });

      const pageWidth = pdf.internal.pageSize.getWidth();
      const pageHeight = pdf.internal.pageSize.getHeight();
      const horizontalMargin = 72;
      const verticalMargin = 72;
      const contentWidth = Math.max(1, pageWidth - horizontalMargin * 2);
      const contentHeight = Math.max(1, pageHeight - verticalMargin * 2);
      const pageHeightPx = Math.floor((contentHeight * canvas.width) / contentWidth);
      const pageBreakPaddingPx = Math.ceil(lineHeightPx);
      const usablePageHeightPx = Math.max(lineHeightPx, pageHeightPx - pageBreakPaddingPx);
      const safePageHeightPx = Math.max(
        lineHeightPx,
        usablePageHeightPx - (usablePageHeightPx % lineHeightPx)
      );

      // Align page slices to line height to reduce mid-line clipping.
      let offsetY = 0;
      let pageIndex = 0;
      while (offsetY < canvas.height) {
        const sliceHeight = Math.min(safePageHeightPx, canvas.height - offsetY);
        const pageCanvas = document.createElement('canvas');
        pageCanvas.width = canvas.width;
        pageCanvas.height = sliceHeight;

        const ctx = pageCanvas.getContext('2d');
        if (!ctx) {
          throw new Error('Failed to render PDF page.');
        }

        ctx.drawImage(
          canvas,
          0,
          offsetY,
          canvas.width,
          sliceHeight,
          0,
          0,
          canvas.width,
          sliceHeight
        );

        const pageData = pageCanvas.toDataURL('image/png');
        if (pageIndex > 0) {
          pdf.addPage();
        }
        const pageImgHeight = (sliceHeight * contentWidth) / canvas.width;
        pdf.addImage(
          pageData,
          'PNG',
          horizontalMargin,
          verticalMargin,
          contentWidth,
          pageImgHeight
        );

        offsetY += sliceHeight;
        pageIndex += 1;
      }

      pdf.save(buildPdfFilename());
      showToast('success', 'Cover letter saved as PDF!');
    } catch {
      showToast('error', 'Failed to save cover letter as PDF');
    } finally {
      setIsExportingPdf(false);
    }
  };

  const handleDownloadCombinedPdf = async () => {
    setIsDownloadingCombinedPdf(true);
    try {
      const { blob, fileName } = await coverLetterService.downloadCombinedPdf(applicationId);
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
      showToast('success', 'Combined PDF downloaded!');
    } catch (error) {
      showToast('error', error instanceof Error ? error.message : 'Failed to download combined PDF');
    } finally {
      setIsDownloadingCombinedPdf(false);
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
            <button
              className="btn-metal"
              onClick={handleSavePdf}
              disabled={isExportingPdf}
              title="Save cover letter as PDF"
            >
              {isExportingPdf ? 'SAVING PDF...' : 'SAVE PDF'}
            </button>
            <button
              className="btn-metal"
              onClick={handleDownloadCombinedPdf}
              disabled={isDownloadingCombinedPdf}
              title="Uses the resume HTML saved in the profile section to render a combined cover letter + resume PDF (no AI API involved)."
            >
              {isDownloadingCombinedPdf ? 'DOWNLOADING...' : 'COMBINED PDF'}
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
              ref={previewRef}
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
