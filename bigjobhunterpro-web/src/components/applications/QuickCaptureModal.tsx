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
  companyName: string;
  roleTitle: string;
  sourceName: string;
  sourceUrl: string;
}

interface FormErrors {
  companyName?: string;
  roleTitle?: string;
  sourceName?: string;
  sourceUrl?: string;
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
    companyName: '',
    roleTitle: '',
    sourceName: '',
    sourceUrl: '',
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
      setFormData({ companyName: '', roleTitle: '', sourceName: '', sourceUrl: '' });
      setErrors({});
    }
  }, [isOpen]);

  const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    // Clear error when user starts typing
    if (errors[name as keyof FormErrors]) {
      setErrors(prev => ({ ...prev, [name]: undefined }));
    }
  };

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    if (!formData.companyName.trim()) {
      newErrors.companyName = 'Company name is required';
    } else if (formData.companyName.length > 200) {
      newErrors.companyName = 'Company name cannot exceed 200 characters';
    }

    if (!formData.roleTitle.trim()) {
      newErrors.roleTitle = 'Role title is required';
    } else if (formData.roleTitle.length > 200) {
      newErrors.roleTitle = 'Role title cannot exceed 200 characters';
    }

    if (!formData.sourceName.trim()) {
      newErrors.sourceName = 'Source is required';
    } else if (formData.sourceName.length > 100) {
      newErrors.sourceName = 'Source cannot exceed 100 characters';
    }

    if (formData.sourceUrl && formData.sourceUrl.length > 500) {
      newErrors.sourceUrl = 'URL cannot exceed 500 characters';
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
        companyName: formData.companyName.trim(),
        roleTitle: formData.roleTitle.trim(),
        sourceName: formData.sourceName.trim(),
        sourceUrl: formData.sourceUrl.trim() || undefined,
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
          id="companyName"
          name="companyName"
          label="COMPANY"
          placeholder="Enter company name"
          value={formData.companyName}
          onChange={handleChange}
          error={errors.companyName}
          disabled={isSubmitting}
        />

        <FormInput
          id="roleTitle"
          name="roleTitle"
          label="ROLE"
          placeholder="Enter role title"
          value={formData.roleTitle}
          onChange={handleChange}
          error={errors.roleTitle}
          disabled={isSubmitting}
        />

        <FormInput
          id="sourceName"
          name="sourceName"
          label="SOURCE"
          placeholder="e.g., LinkedIn, Indeed, Referral"
          value={formData.sourceName}
          onChange={handleChange}
          error={errors.sourceName}
          disabled={isSubmitting}
        />

        <FormInput
          id="sourceUrl"
          name="sourceUrl"
          label="JOB URL (OPTIONAL)"
          placeholder="https://..."
          type="url"
          value={formData.sourceUrl}
          onChange={handleChange}
          error={errors.sourceUrl}
          disabled={isSubmitting}
        />

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
