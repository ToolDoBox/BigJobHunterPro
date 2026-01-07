import { useEffect, useMemo, useState, type ChangeEvent } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import StatusBadge from '@/components/applications/StatusBadge';
import FormInput from '@/components/forms/FormInput';
import Modal from '@/components/ui/Modal';
import applicationsService from '@/services/applications';
import { useToast } from '@/context/ToastContext';
import { formatRelativeDate } from '@/utils/date';
import { useAuth } from '@/hooks/useAuth';
import { TimelineView } from '@/components/applications/TimelineView';
import type {
  ApplicationDetail,
  UpdateApplicationRequest
} from '@/types/application';

const WORK_MODE_OPTIONS = ['Remote', 'Hybrid', 'Onsite', 'Unknown'];

interface FormState {
  companyName: string;
  roleTitle: string;
  sourceName: string;
  sourceUrl: string;
  status: string;
  workMode: string;
  location: string;
  salaryMin: string;
  salaryMax: string;
  jobDescription: string;
  requiredSkillsInput: string;
  niceToHaveSkillsInput: string;
  rawPageContent: string;
}

interface FormErrors {
  companyName?: string;
  roleTitle?: string;
  sourceName?: string;
  sourceUrl?: string;
  status?: string;
  workMode?: string;
  location?: string;
  salaryMin?: string;
  salaryMax?: string;
  jobDescription?: string;
  requiredSkills?: string;
  niceToHaveSkills?: string;
  rawPageContent?: string;
  general?: string;
}

const emptyFormState: FormState = {
  companyName: '',
  roleTitle: '',
  sourceName: '',
  sourceUrl: '',
  status: '',
  workMode: '',
  location: '',
  salaryMin: '',
  salaryMax: '',
  jobDescription: '',
  requiredSkillsInput: '',
  niceToHaveSkillsInput: '',
  rawPageContent: '',
};

const buildFormState = (application: ApplicationDetail): FormState => ({
  companyName: application.companyName ?? '',
  roleTitle: application.roleTitle ?? '',
  sourceName: application.sourceName ?? '',
  sourceUrl: application.sourceUrl ?? '',
  status: application.status ?? '',
  workMode: application.workMode ?? '',
  location: application.location ?? '',
  salaryMin: application.salaryMin?.toString() ?? '',
  salaryMax: application.salaryMax?.toString() ?? '',
  jobDescription: application.jobDescription ?? '',
  requiredSkillsInput: application.requiredSkills?.join(', ') ?? '',
  niceToHaveSkillsInput: application.niceToHaveSkills?.join(', ') ?? '',
  rawPageContent: application.rawPageContent ?? '',
});

const parseSkills = (input: string): string[] => {
  if (!input.trim()) return [];
  return input
    .split(',')
    .map((skill) => skill.trim())
    .filter(Boolean);
};

const parseNumber = (value: string): number | null => {
  const parsed = Number.parseInt(value, 10);
  return Number.isFinite(parsed) ? parsed : null;
};

const validateForm = (formData: FormState): FormErrors => {
  const errors: FormErrors = {};

  if (!formData.companyName.trim()) {
    errors.companyName = 'Company name is required';
  } else if (formData.companyName.length > 200) {
    errors.companyName = 'Company name cannot exceed 200 characters';
  }

  if (!formData.roleTitle.trim()) {
    errors.roleTitle = 'Role title is required';
  } else if (formData.roleTitle.length > 200) {
    errors.roleTitle = 'Role title cannot exceed 200 characters';
  }

  if (!formData.sourceName.trim()) {
    errors.sourceName = 'Source name is required';
  } else if (formData.sourceName.length > 100) {
    errors.sourceName = 'Source name cannot exceed 100 characters';
  }

  // Status is now computed from timeline events, no validation needed

  if (formData.workMode && !WORK_MODE_OPTIONS.includes(formData.workMode)) {
    errors.workMode = 'Work mode must be a valid option';
  }

  if (formData.location.length > 200) {
    errors.location = 'Location cannot exceed 200 characters';
  }

  if (formData.sourceUrl && formData.sourceUrl.length > 500) {
    errors.sourceUrl = 'URL cannot exceed 500 characters';
  }

  if (formData.sourceUrl) {
    try {
      new URL(formData.sourceUrl);
    } catch {
      errors.sourceUrl = 'URL must be a valid URL';
    }
  }

  const salaryMin = formData.salaryMin ? parseNumber(formData.salaryMin) : null;
  const salaryMax = formData.salaryMax ? parseNumber(formData.salaryMax) : null;

  if (formData.salaryMin && (salaryMin === null || salaryMin < 0 || salaryMin > 1000)) {
    errors.salaryMin = 'Salary minimum must be between 0 and 1000';
  }

  if (formData.salaryMax && (salaryMax === null || salaryMax < 0 || salaryMax > 1000)) {
    errors.salaryMax = 'Salary maximum must be between 0 and 1000';
  }

  if (salaryMin !== null && salaryMax !== null && salaryMin > salaryMax) {
    errors.salaryMin = 'Salary minimum cannot exceed salary maximum';
  }

  if (formData.jobDescription.length > 4000) {
    errors.jobDescription = 'Job description cannot exceed 4000 characters';
  }

  return errors;
};

export default function ApplicationDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { showToast } = useToast();
  const { refreshUser } = useAuth();

  const [application, setApplication] = useState<ApplicationDetail | null>(null);
  const [formData, setFormData] = useState<FormState>(emptyFormState);
  const [initialFormData, setInitialFormData] = useState<FormState>(emptyFormState);
  const [errors, setErrors] = useState<FormErrors>({});
  const [isEditing, setIsEditing] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [notFound, setNotFound] = useState(false);
  const [loadError, setLoadError] = useState<string | null>(null);

  const hasChanges = useMemo(
    () => JSON.stringify(formData) !== JSON.stringify(initialFormData),
    [formData, initialFormData]
  );

  const fetchApplication = async () => {
    if (!id) {
      setNotFound(true);
      setIsLoading(false);
      return;
    }

    setIsLoading(true);
    setLoadError(null);
    setNotFound(false);

    try {
      const result = await applicationsService.getApplication(id);
      setApplication(result);
      const nextFormState = buildFormState(result);
      setFormData(nextFormState);
      setInitialFormData(nextFormState);
      setIsEditing(false);
    } catch (error) {
      const err = error as Error & { code?: string };
      if (err.code === 'NOT_FOUND' || err.message === 'NOT_FOUND') {
        setNotFound(true);
      } else {
        setLoadError(err.message ?? 'Unable to load application.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchApplication();
  }, [id]);

  const handleBack = () => {
    if (hasChanges && !window.confirm('Discard unsaved changes and leave this page?')) {
      return;
    }
    navigate('/app/applications');
  };

  const handleEditToggle = () => {
    if (isEditing) {
      setFormData(initialFormData);
      setErrors({});
      setIsEditing(false);
    } else {
      setIsEditing(true);
    }
  };

  const handleChange = (
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
  ) => {
    const { name, value } = event.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    if (errors[name as keyof FormErrors]) {
      setErrors((prev) => ({ ...prev, [name]: undefined }));
    }
    if (name === 'requiredSkillsInput' && errors.requiredSkills) {
      setErrors((prev) => ({ ...prev, requiredSkills: undefined }));
    }
    if (name === 'niceToHaveSkillsInput' && errors.niceToHaveSkills) {
      setErrors((prev) => ({ ...prev, niceToHaveSkills: undefined }));
    }
  };

  const handleSave = async () => {
    if (!id) return;

    const validationErrors = validateForm(formData);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    const payload: UpdateApplicationRequest = {
      companyName: formData.companyName.trim(),
      roleTitle: formData.roleTitle.trim(),
      sourceName: formData.sourceName.trim(),
      sourceUrl: formData.sourceUrl.trim() || undefined,
      // status is computed from timeline events, not updated directly
      status: application?.status || 'Applied', // Send current computed status
      workMode: formData.workMode || undefined,
      location: formData.location.trim() || undefined,
      salaryMin: formData.salaryMin ? parseNumber(formData.salaryMin) : null,
      salaryMax: formData.salaryMax ? parseNumber(formData.salaryMax) : null,
      jobDescription: formData.jobDescription.trim() || undefined,
      requiredSkills: parseSkills(formData.requiredSkillsInput),
      niceToHaveSkills: parseSkills(formData.niceToHaveSkillsInput),
      rawPageContent: formData.rawPageContent.trim() || undefined,
    };

    setIsSaving(true);
    setErrors({});

    try {
      const updated = await applicationsService.updateApplication(id, payload);
      setApplication(updated);
      const nextFormState = buildFormState(updated);
      setFormData(nextFormState);
      setInitialFormData(nextFormState);
      setIsEditing(false);
      await refreshUser();
      showToast('success', 'Application updated successfully!');
    } catch (error) {
      const err = error as { errors?: Record<string, string[]> };
      if (err.errors) {
        const nextErrors: FormErrors = {};
        Object.entries(err.errors).forEach(([key, messages]) => {
          nextErrors[key as keyof FormErrors] = messages[0];
        });
        setErrors(nextErrors);
      } else {
        setErrors({
          general: error instanceof Error
            ? error.message
            : 'We could not save your changes. Please try again.'
        });
      }
    } finally {
      setIsSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!id) return;
    setIsDeleting(true);
    try {
      await applicationsService.deleteApplication(id);
      await refreshUser();
      showToast('success', 'Application deleted.');
      navigate('/app/applications');
    } catch (error) {
      const err = error as Error;
      showToast('error', err.message ?? 'Unable to delete application.');
    } finally {
      setIsDeleting(false);
      setShowDeleteConfirm(false);
    }
  };

  if (isLoading) {
    return (
      <div className="space-y-6 animate-fade-in motion-reduce:animate-none">
        <div className="metal-panel metal-panel-orange">
          <div className="metal-panel-screws" />
          <div className="h-6 w-40 bg-metal-light/60 rounded animate-pulse motion-reduce:animate-none" />
          <div className="mt-4 h-4 w-64 bg-metal-light/40 rounded animate-pulse motion-reduce:animate-none" />
        </div>
        <div className="metal-panel">
          <div className="metal-panel-screws" />
          <div className="space-y-4">
            <div className="h-4 w-32 bg-metal-light/50 rounded animate-pulse motion-reduce:animate-none" />
            <div className="h-10 w-full bg-metal-light/40 rounded animate-pulse motion-reduce:animate-none" />
            <div className="h-10 w-full bg-metal-light/40 rounded animate-pulse motion-reduce:animate-none" />
            <div className="h-24 w-full bg-metal-light/30 rounded animate-pulse motion-reduce:animate-none" />
          </div>
        </div>
      </div>
    );
  }

  if (notFound) {
    return (
      <div className="metal-panel text-center">
        <div className="metal-panel-screws" />
        <h2 className="font-arcade text-xl text-amber mb-4">TARGET LOST</h2>
        <p className="text-gray-400 mb-6">
          We could not find that application. It may have been removed.
        </p>
        <button className="btn-metal-primary" onClick={handleBack}>
          BACK TO ARMORY
        </button>
      </div>
    );
  }

  if (loadError) {
    return (
      <div className="metal-panel text-center">
        <div className="metal-panel-screws" />
        <h2 className="font-arcade text-xl text-amber mb-4">SYSTEM JAMMED</h2>
        <p className="text-gray-400 mb-6">{loadError}</p>
        <button className="btn-metal-primary" onClick={() => window.location.reload()}>
          RETRY
        </button>
      </div>
    );
  }

  if (!application) {
    return null;
  }

  const companyDisplay = application.companyName?.trim() || 'Pending company';
  const roleDisplay = application.roleTitle?.trim() || 'Pending role';
  const sourceDisplay = application.sourceName?.trim() || 'Pending source';

  const statusDisplay = isEditing && formData.status ? formData.status : application.status;

  return (
    <div className="space-y-6">
      <div className="metal-panel metal-panel-orange">
        <div className="metal-panel-screws" />
        <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
          <div className="space-y-2">
            <button className="btn-metal" onClick={handleBack}>
              BACK TO ARMORY
            </button>
            <h1
              className="font-arcade text-xl md:text-2xl text-blaze truncate max-w-[280px] md:max-w-none"
              title={companyDisplay}
            >
              {companyDisplay}
            </h1>
            <p
              className="text-gray-300 text-sm truncate max-w-[280px] md:max-w-none"
              title={roleDisplay}
            >
              {roleDisplay} · {sourceDisplay}
            </p>
          </div>
          <div className="flex flex-col gap-2 md:items-end">
            <StatusBadge status={statusDisplay} />
            <div className="text-xs text-gray-400">
              POINTS: <span className="text-amber font-semibold">{application.points}</span>
            </div>
            <div className="text-xs text-gray-400">
              LOGGED {formatRelativeDate(application.createdDate)}
            </div>
          </div>
        </div>
      </div>

      <div className="metal-panel">
        <div className="metal-panel-screws" />
        <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between mb-6">
          <div>
            <h2 className="font-arcade text-sm text-amber">APPLICATION DETAIL</h2>
            <p className="text-xs text-gray-400">
              AI STATUS: {application.aiParsingStatus || 'Pending'}
            </p>
          </div>
          <div className="flex flex-wrap gap-2">
            {!isEditing && (
              <button className="btn-metal" onClick={handleEditToggle}>
                EDIT
              </button>
            )}
            <button
              className="btn-metal"
              onClick={() => setShowDeleteConfirm(true)}
              disabled={isDeleting}
            >
              {isDeleting ? 'DELETING...' : 'DELETE'}
            </button>
          </div>
        </div>

        <div className="grid gap-6 lg:grid-cols-2">
          <div>
            <h3 className="font-arcade text-xs text-amber mb-3">CORE INTEL</h3>
            <FormInput
              id="companyName"
              name="companyName"
              label="COMPANY"
              placeholder="Enter company name"
              value={formData.companyName}
              onChange={handleChange}
              error={errors.companyName}
              disabled={!isEditing || isSaving}
            />
            <FormInput
              id="roleTitle"
              name="roleTitle"
              label="ROLE TITLE"
              placeholder="Enter role title"
              value={formData.roleTitle}
              onChange={handleChange}
              error={errors.roleTitle}
              disabled={!isEditing || isSaving}
            />
            <FormInput
              id="sourceName"
              name="sourceName"
              label="SOURCE NAME"
              placeholder="LinkedIn, Indeed, Referral"
              value={formData.sourceName}
              onChange={handleChange}
              error={errors.sourceName}
              disabled={!isEditing || isSaving}
            />
            <FormInput
              id="sourceUrl"
              name="sourceUrl"
              label="SOURCE URL"
              placeholder="https://..."
              type="url"
              value={formData.sourceUrl}
              onChange={handleChange}
              error={errors.sourceUrl}
              disabled={!isEditing || isSaving}
            />

            {/* Status is now computed from timeline events */}
            <div className="mb-4">
              <label className="block font-arcade text-xs text-amber mb-2">
                CURRENT STATUS
              </label>
              <div className="input-arcade flex items-center gap-2" style={{ opacity: 0.7 }}>
                <StatusBadge status={application?.status || 'Applied'} />
                <span className="text-xs text-gray-400">(Computed from timeline)</span>
              </div>
            </div>
          </div>

          <div>
            <h3 className="font-arcade text-xs text-amber mb-3">WORK DETAILS</h3>
            <div className="mb-4">
              <label htmlFor="workMode" className="block font-arcade text-xs text-amber mb-2">
                WORK MODE
              </label>
              <select
                id="workMode"
                name="workMode"
                className={`input-arcade ${errors.workMode ? 'input-arcade-error' : ''}`}
                value={formData.workMode}
                onChange={handleChange}
                disabled={!isEditing || isSaving}
              >
                <option value="">Select work mode</option>
                {WORK_MODE_OPTIONS.map((option) => (
                  <option key={option} value={option}>
                    {option}
                  </option>
                ))}
              </select>
              {errors.workMode && (
                <p className="mt-1 text-sm text-red-400 font-medium" role="alert">
                  {errors.workMode}
                </p>
              )}
            </div>

            <FormInput
              id="location"
              name="location"
              label="LOCATION"
              placeholder="City, State, Country"
              value={formData.location}
              onChange={handleChange}
              error={errors.location}
              disabled={!isEditing || isSaving}
            />

            <div className="grid gap-4 md:grid-cols-2">
              <FormInput
                id="salaryMin"
                name="salaryMin"
                label="SALARY MIN (K)"
                type="number"
                placeholder="120"
                value={formData.salaryMin}
                onChange={handleChange}
                error={errors.salaryMin}
                disabled={!isEditing || isSaving}
                min={0}
                max={1000}
                step={1}
              />
              <FormInput
                id="salaryMax"
                name="salaryMax"
                label="SALARY MAX (K)"
                type="number"
                placeholder="150"
                value={formData.salaryMax}
                onChange={handleChange}
                error={errors.salaryMax}
                disabled={!isEditing || isSaving}
                min={0}
                max={1000}
                step={1}
              />
            </div>
          </div>
        </div>

        <div className="divider-metal my-6" />

        <div className="grid gap-6 lg:grid-cols-2">
          <div>
            <h3 className="font-arcade text-xs text-amber mb-3">JOB DESCRIPTION</h3>
            <textarea
              id="jobDescription"
              name="jobDescription"
              rows={6}
              className={`input-arcade ${errors.jobDescription ? 'input-arcade-error' : ''}`}
              placeholder="Paste the role summary..."
              value={formData.jobDescription}
              onChange={handleChange}
              disabled={!isEditing || isSaving}
              aria-label="Job description"
            />
            {errors.jobDescription && (
              <p className="mt-1 text-sm text-red-400 font-medium" role="alert">
                {errors.jobDescription}
              </p>
            )}
          </div>

          <div>
            <h3 className="font-arcade text-xs text-amber mb-3">SKILLS</h3>
            <textarea
              id="requiredSkillsInput"
              name="requiredSkillsInput"
              rows={3}
              className={`input-arcade ${errors.requiredSkills ? 'input-arcade-error' : ''}`}
              placeholder="Required skills (comma separated)"
              value={formData.requiredSkillsInput}
              onChange={handleChange}
              disabled={!isEditing || isSaving}
              aria-label="Required skills"
            />
            {errors.requiredSkills && (
              <p className="mt-1 text-sm text-red-400 font-medium" role="alert">
                {errors.requiredSkills}
              </p>
            )}
            <textarea
              id="niceToHaveSkillsInput"
              name="niceToHaveSkillsInput"
              rows={3}
              className={`input-arcade mt-4 ${errors.niceToHaveSkills ? 'input-arcade-error' : ''}`}
              placeholder="Nice-to-have skills (comma separated)"
              value={formData.niceToHaveSkillsInput}
              onChange={handleChange}
              disabled={!isEditing || isSaving}
              aria-label="Nice-to-have skills"
            />
            {errors.niceToHaveSkills && (
              <p className="mt-1 text-sm text-red-400 font-medium" role="alert">
                {errors.niceToHaveSkills}
              </p>
            )}
          </div>
        </div>

        <div className="divider-metal my-6" />

        <div>
          <h3 className="font-arcade text-xs text-amber mb-3">RAW CAPTURE</h3>
          <textarea
            id="rawPageContent"
            name="rawPageContent"
            rows={6}
            className={`input-arcade ${errors.rawPageContent ? 'input-arcade-error' : ''}`}
            placeholder="Paste the original capture here..."
            value={formData.rawPageContent}
            onChange={handleChange}
            disabled={!isEditing || isSaving}
            aria-label="Raw capture"
          />
          {errors.rawPageContent && (
            <p className="mt-1 text-sm text-red-400 font-medium" role="alert">
              {errors.rawPageContent}
            </p>
          )}
        </div>

        <div className="divider-metal my-6" />

        {/* Timeline Events Section */}
        {application && (
          <TimelineView
            applicationId={application.id}
            events={application.timelineEvents || []}
            onEventsUpdated={fetchApplication}
          />
        )}

        {errors.general && (
          <p className="text-red-400 text-sm mt-4">{errors.general}</p>
        )}

        {isEditing && (
          <div className="mt-6 flex flex-col gap-3 md:flex-row md:justify-end">
            <button
              type="button"
              className="btn-metal"
              onClick={handleEditToggle}
              disabled={isSaving}
            >
              CANCEL
            </button>
            <button
              type="button"
              className="btn-metal-primary"
              onClick={handleSave}
              disabled={isSaving}
            >
              {isSaving ? 'SAVING...' : 'SAVE CHANGES'}
            </button>
          </div>
        )}
      </div>

      <Modal
        isOpen={showDeleteConfirm}
        onClose={() => setShowDeleteConfirm(false)}
        title="DELETE APPLICATION"
      >
        <p className="text-gray-300 mb-6">
          This will permanently remove the application and adjust your points.
        </p>
        <div className="flex gap-3">
          <button
            type="button"
            className="btn-metal flex-1"
            onClick={() => setShowDeleteConfirm(false)}
            disabled={isDeleting}
          >
            NEVERMIND
          </button>
          <button
            type="button"
            className="btn-metal-primary flex-1"
            onClick={handleDelete}
            disabled={isDeleting}
          >
            {isDeleting ? 'DELETING...' : 'CONFIRM'}
          </button>
        </div>
      </Modal>
    </div>
  );
}
