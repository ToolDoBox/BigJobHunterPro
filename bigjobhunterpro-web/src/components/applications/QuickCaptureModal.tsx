import { useState, useEffect, useRef, type FormEvent, type ChangeEvent } from 'react';
import Modal from '@/components/ui/Modal';
import FormInput from '@/components/forms/FormInput';
import applicationsService from '@/services/applications';
import { useToast } from '@/context/ToastContext';
import { useAuth } from '@/hooks/useAuth';

interface QuickCaptureModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess?: () => void;
}

interface FormData {
  sourceUrl: string;
  rawPageContent: string;
}

interface FormErrors {
  sourceUrl?: string;
  rawPageContent?: string;
  general?: string;
}

export default function QuickCaptureModal({
  isOpen,
  onClose,
  onSuccess
}: QuickCaptureModalProps) {
  const { refreshUser } = useAuth();
  const { showToast } = useToast();
  const firstInputRef = useRef<HTMLInputElement>(null);

  const [formData, setFormData] = useState<FormData>({
    sourceUrl: '',
    rawPageContent: '',
  });

  const [errors, setErrors] = useState<FormErrors>({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Auto-focus first input when modal opens
  useEffect(() => {
    if (isOpen) {
      setTimeout(() => firstInputRef.current?.focus(), 100);
    }
  }, [isOpen]);

  // Reset form when modal closes
  useEffect(() => {
    if (!isOpen) {
      setFormData({ sourceUrl: '', rawPageContent: '' });
      setErrors({});
    }
  }, [isOpen]);

  const handleChange = (e: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    // Clear error when user starts typing
    if (errors[name as keyof FormErrors]) {
      setErrors(prev => ({ ...prev, [name]: undefined }));
    }
  };

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    if (formData.sourceUrl && formData.sourceUrl.length > 500) {
      newErrors.sourceUrl = 'URL cannot exceed 500 characters';
    }

    if (formData.sourceUrl) {
      try {
        new URL(formData.sourceUrl);
      } catch {
        newErrors.sourceUrl = 'URL must be a valid URL';
      }
    }

    if (!formData.rawPageContent.trim()) {
      newErrors.rawPageContent = 'Job page content is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    if (!validateForm()) return;

    setIsSubmitting(true);
    setErrors({});

    try {
      const result = await applicationsService.createApplication({
        sourceUrl: formData.sourceUrl.trim() || undefined,
        rawPageContent: formData.rawPageContent.trim(),
      });

      // Refresh user to update points in header
      await refreshUser();

      // Show success toast with points
      showToast('success', 'Hunt logged successfully!', result.points);

      // Close modal and trigger callback
      onClose();
      onSuccess?.();
    } catch (error: unknown) {
      const err = error as { errors?: Record<string, string[]> };
      if (err.errors) {
        const newErrors: FormErrors = {};
        Object.entries(err.errors).forEach(([key, messages]) => {
          newErrors[key as keyof FormErrors] = messages[0];
        });
        setErrors(newErrors);
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="LOG A HUNT">
      <form onSubmit={handleSubmit}>
        <FormInput
          ref={firstInputRef}
          id="sourceUrl"
          name="sourceUrl"
          label="JOB URL"
          placeholder="https://..."
          onChange={handleChange}
          type="url"
          value={formData.sourceUrl}
          error={errors.sourceUrl}
          disabled={isSubmitting}
        />

        <div className="mb-4">
          <label
            htmlFor="rawPageContent"
            className="block font-arcade text-xs text-amber mb-2"
          >
            JOB PAGE CONTENT
          </label>
          <textarea
            id="rawPageContent"
            name="rawPageContent"
            rows={6}
            className={`input-arcade ${errors.rawPageContent ? 'input-arcade-error' : ''}`}
            placeholder="Paste the full job listing content here..."
            value={formData.rawPageContent}
            onChange={handleChange}
            disabled={isSubmitting}
            aria-describedby={errors.rawPageContent ? 'rawPageContent-error' : undefined}
            aria-invalid={errors.rawPageContent ? 'true' : 'false'}
          />
          {errors.rawPageContent && (
            <p
              id="rawPageContent-error"
              className="mt-1 text-sm text-red-400 font-medium flex items-start gap-1"
              role="alert"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 20 20"
                fill="currentColor"
                className="w-4 h-4 mt-0.5 flex-shrink-0"
              >
                <path
                  fillRule="evenodd"
                  d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-8-5a.75.75 0 01.75.75v4.5a.75.75 0 01-1.5 0v-4.5A.75.75 0 0110 5zm0 10a1 1 0 100-2 1 1 0 000 2z"
                  clipRule="evenodd"
                />
              </svg>
              <span>{errors.rawPageContent}</span>
            </p>
          )}
        </div>

        {errors.general && (
          <p className="text-red-500 text-sm mb-4">{errors.general}</p>
        )}

        <div className="flex gap-4 mt-6">
          <button
            type="button"
            onClick={onClose}
            className="btn-metal flex-1"
            disabled={isSubmitting}
          >
            NEVERMIND
          </button>
          <button
            type="submit"
            className="btn-metal-primary flex-1"
            disabled={isSubmitting}
          >
            {isSubmitting ? 'LOGGING...' : 'LOCK IT IN'}
          </button>
        </div>
      </form>
    </Modal>
  );
}
