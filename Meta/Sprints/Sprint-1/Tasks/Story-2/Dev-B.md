# realemmetts Task List - Frontend (Dev B)

**Story:** Quick Capture + Points (5 SP)
**Sprint:** 1 | Jan 4-17, 2026
**Tech Stack:** React 18, TypeScript, Vite, Tailwind CSS
**Branch:** `feature/story2-frontend-quick-capture`

---

## PHASE 0: API CONTRACT (SYNC POINT - DO FIRST)

Before coding, confirm these contracts with cadleta:

### Applications Endpoint (cadleta Builds This)
| Method | Endpoint | Request Body | Response (201) |
|--------|----------|--------------|----------------|
| POST | /api/applications | `{ companyName, roleTitle, sourceName, sourceUrl? }` | `{ id, companyName, roleTitle, sourceName, sourceUrl, status, points, totalPoints, createdDate }` |

### Request Body
```typescript
interface CreateApplicationRequest {
  companyName: string;    // required, max 200 chars
  roleTitle: string;      // required, max 200 chars
  sourceName: string;     // required, max 100 chars
  sourceUrl?: string;     // optional, max 500 chars, valid URL
}
```

### Response (201 Created)
```typescript
interface CreateApplicationResponse {
  id: string;
  companyName: string;
  roleTitle: string;
  sourceName: string;
  sourceUrl: string | null;
  status: 'Applied';
  points: number;         // points earned (1 for Applied)
  totalPoints: number;    // user's updated total
  createdDate: string;    // ISO 8601
}
```

### Error Response (400 Validation)
```typescript
interface ValidationError {
  type: string;
  title: string;
  status: 400;
  errors: Record<string, string[]>;
}
```

---

## YOUR TASKS

### B1. Create Reusable Modal Component
**Depends on:** Story 1 frontend complete
- [ ] Create Modal component with backdrop
- [ ] Add open/close animation (fade + scale)
- [ ] Handle Escape key to close
- [ ] Handle click outside to close
- [ ] Add focus trap for accessibility
- [ ] Style with metal-panel theme

**Files to create:**
- `src/components/ui/Modal.tsx`

**Modal Component:**
```typescript
// src/components/ui/Modal.tsx
import { useEffect, useRef, type ReactNode } from 'react';

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
  const modalRef = useRef<HTMLDivElement>(null);

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

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      {/* Backdrop */}
      <div
        className="absolute inset-0 bg-black/70 backdrop-blur-sm"
        onClick={onClose}
      />

      {/* Modal */}
      <div
        ref={modalRef}
        role="dialog"
        aria-modal="true"
        aria-labelledby="modal-title"
        tabIndex={-1}
        className="relative metal-panel metal-panel-orange w-full max-w-md mx-4 animate-scale-in"
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
```

---

### B2. Create Toast Notification System
**Depends on:** None (can be parallel with B1)
- [ ] Create ToastContext for global toast state
- [ ] Create Toast component with success/error variants
- [ ] Add auto-dismiss after 4 seconds
- [ ] Add manual dismiss button
- [ ] Style with retro-arcade theme
- [ ] Show points earned on success

**Files to create:**
- `src/context/ToastContext.tsx`
- `src/components/ui/Toast.tsx`
- `src/components/ui/ToastContainer.tsx`

**ToastContext:**
```typescript
// src/context/ToastContext.tsx
import { createContext, useContext, useState, useCallback, type ReactNode } from 'react';

export type ToastType = 'success' | 'error' | 'info';

interface Toast {
  id: string;
  type: ToastType;
  message: string;
  points?: number;
}

interface ToastContextType {
  toasts: Toast[];
  showToast: (type: ToastType, message: string, points?: number) => void;
  dismissToast: (id: string) => void;
}

const ToastContext = createContext<ToastContextType | null>(null);

export function ToastProvider({ children }: { children: ReactNode }) {
  const [toasts, setToasts] = useState<Toast[]>([]);

  const showToast = useCallback((type: ToastType, message: string, points?: number) => {
    const id = crypto.randomUUID();
    setToasts(prev => [...prev, { id, type, message, points }]);

    // Auto-dismiss after 4 seconds
    setTimeout(() => {
      setToasts(prev => prev.filter(t => t.id !== id));
    }, 4000);
  }, []);

  const dismissToast = useCallback((id: string) => {
    setToasts(prev => prev.filter(t => t.id !== id));
  }, []);

  return (
    <ToastContext.Provider value={{ toasts, showToast, dismissToast }}>
      {children}
    </ToastContext.Provider>
  );
}

export function useToast() {
  const context = useContext(ToastContext);
  if (!context) throw new Error('useToast must be used within ToastProvider');
  return context;
}
```

**Toast Component:**
```typescript
// src/components/ui/Toast.tsx
import type { ToastType } from '@/context/ToastContext';

interface ToastProps {
  type: ToastType;
  message: string;
  points?: number;
  onDismiss: () => void;
}

export default function Toast({ type, message, points, onDismiss }: ToastProps) {
  const borderColor = type === 'success'
    ? 'border-terminal'
    : type === 'error'
    ? 'border-red-500'
    : 'border-amber';

  return (
    <div className={`metal-panel ${borderColor} border-l-4 flex items-center gap-4 min-w-[300px] animate-slide-in-right`}>
      <div className="flex-1">
        <p className="text-white font-medium">{message}</p>
        {points !== undefined && (
          <p className="font-arcade text-terminal text-sm mt-1">
            +{points} POINT{points !== 1 ? 'S' : ''}!
          </p>
        )}
      </div>
      <button
        onClick={onDismiss}
        className="text-gray-400 hover:text-white font-arcade text-xs"
        aria-label="Dismiss"
      >
        X
      </button>
    </div>
  );
}
```

**ToastContainer:**
```typescript
// src/components/ui/ToastContainer.tsx
import { useToast } from '@/context/ToastContext';
import Toast from './Toast';

export default function ToastContainer() {
  const { toasts, dismissToast } = useToast();

  if (toasts.length === 0) return null;

  return (
    <div className="fixed top-4 right-4 z-50 space-y-2">
      {toasts.map(toast => (
        <Toast
          key={toast.id}
          type={toast.type}
          message={toast.message}
          points={toast.points}
          onDismiss={() => dismissToast(toast.id)}
        />
      ))}
    </div>
  );
}
```

---

### B3. Create Application Types and API Service
**Depends on:** None (can be parallel)
- [ ] Create TypeScript interfaces for application DTOs
- [ ] Create applications service with createApplication function
- [ ] Handle API errors consistently

**Files to create:**
- `src/types/application.ts`
- `src/services/applications.ts`

**Application Types:**
```typescript
// src/types/application.ts
export interface CreateApplicationRequest {
  companyName: string;
  roleTitle: string;
  sourceName: string;
  sourceUrl?: string;
}

export interface CreateApplicationResponse {
  id: string;
  companyName: string;
  roleTitle: string;
  sourceName: string;
  sourceUrl: string | null;
  status: string;
  points: number;
  totalPoints: number;
  createdDate: string;
}

export interface ApplicationError {
  type?: string;
  title?: string;
  status?: number;
  errors?: Record<string, string[]>;
}
```

**Applications Service:**
```typescript
// src/services/applications.ts
import axios from 'axios';
import api from './api';
import type {
  CreateApplicationRequest,
  CreateApplicationResponse,
  ApplicationError
} from '@/types/application';

const parseError = (error: unknown): Record<string, string[]> => {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as ApplicationError | undefined;

    if (data?.errors) {
      return data.errors;
    }

    return { general: ['An unexpected error occurred. Please try again.'] };
  }

  return { general: ['Network error. Please check your connection.'] };
};

export const applicationsService = {
  async createApplication(data: CreateApplicationRequest): Promise<CreateApplicationResponse> {
    try {
      const response = await api.post<CreateApplicationResponse>('/api/applications', data);
      return response.data;
    } catch (error) {
      const errors = parseError(error);
      throw { errors };
    }
  },
};

export default applicationsService;
```

---

### B4. Create QuickCaptureModal Component
**Depends on:** B1, B3
- [ ] Create QuickCaptureModal with form inputs
- [ ] Add form state and validation
- [ ] Implement Enter key submit
- [ ] Auto-focus first input on open
- [ ] Add loading state during submission
- [ ] Handle validation errors inline
- [ ] Call refreshUser on success to update points

**Files to create:**
- `src/components/applications/QuickCaptureModal.tsx`

**QuickCaptureModal:**
```typescript
// src/components/applications/QuickCaptureModal.tsx
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
```

---

### B5. Update FormInput to Support Ref Forwarding
**Depends on:** B4
- [ ] Update FormInput to use forwardRef
- [ ] Add name prop support
- [ ] Maintain backwards compatibility

**Files to modify:**
- `src/components/forms/FormInput.tsx`

**Updated FormInput:**
```typescript
import { forwardRef, type InputHTMLAttributes, type ChangeEvent } from 'react';

interface FormInputProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'onChange'> {
  id: string;
  label: string;
  error?: string;
  onChange: (e: ChangeEvent<HTMLInputElement>) => void;
}

const FormInput = forwardRef<HTMLInputElement, FormInputProps>(({
  id,
  label,
  error,
  className = '',
  ...props
}, ref) => {
  return (
    <div className="mb-4">
      <label
        htmlFor={id}
        className="block font-arcade text-xs text-amber mb-2"
      >
        {label}
      </label>

      <input
        ref={ref}
        id={id}
        className={`
          input-metal w-full
          ${error ? 'input-metal-error' : ''}
          ${className}
        `}
        aria-describedby={error ? `${id}-error` : undefined}
        aria-invalid={error ? 'true' : 'false'}
        {...props}
      />

      {error && (
        <p
          id={`${id}-error`}
          className="mt-1 text-sm text-red-500 font-medium"
          role="alert"
        >
          {error}
        </p>
      )}
    </div>
  );
});

FormInput.displayName = 'FormInput';

export default FormInput;
```

---

### B6. Add Modal CSS Animations
**Depends on:** B1
- [ ] Add scale-in animation for modal
- [ ] Add slide-in animation for toasts

**Files to modify:**
- `src/index.css`

**Add to index.css:**
```css
/* Modal and Toast Animations */
@keyframes scale-in {
  from {
    opacity: 0;
    transform: scale(0.95);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

@keyframes slide-in-right {
  from {
    opacity: 0;
    transform: translateX(100%);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

.animate-scale-in {
  animation: scale-in 0.2s ease-out;
}

.animate-slide-in-right {
  animation: slide-in-right 0.3s ease-out;
}
```

---

### B7. Integrate Quick Capture into Dashboard and App
**Depends on:** B2, B4
- [ ] Add QuickCaptureModal to Dashboard
- [ ] Wire up "QUICK CAPTURE" button to open modal
- [ ] Add ToastProvider to app root (App.tsx)
- [ ] Add ToastContainer to AppShell

**Files to modify:**
- `src/pages/Dashboard.tsx`
- `src/App.tsx`
- `src/components/layout/AppShell.tsx`

**Updated Dashboard:**
```typescript
// src/pages/Dashboard.tsx
import { useState } from 'react';
import { useAuth } from '@/hooks/useAuth';
import QuickCaptureModal from '@/components/applications/QuickCaptureModal';

export default function Dashboard() {
  const { user } = useAuth();
  const [isQuickCaptureOpen, setIsQuickCaptureOpen] = useState(false);

  return (
    <div className="space-y-8">
      {/* ... existing welcome banner and stats ... */}

      {/* Quick actions - Metal panel */}
      <div className="metal-panel">
        <div className="metal-panel-screws" />
        <h2 className="font-arcade text-base text-amber mb-4">QUICK ACTIONS</h2>
        <button
          className="btn-metal-primary"
          onClick={() => setIsQuickCaptureOpen(true)}
        >
          + QUICK CAPTURE
        </button>
      </div>

      {/* Quick Capture Modal */}
      <QuickCaptureModal
        isOpen={isQuickCaptureOpen}
        onClose={() => setIsQuickCaptureOpen(false)}
        onSuccess={() => {
          // Optionally refresh applications list (Story 3)
        }}
      />
    </div>
  );
}
```

**Updated App.tsx:**
```typescript
// Wrap with ToastProvider
import { ToastProvider } from '@/context/ToastContext';

function App() {
  return (
    <AuthProvider>
      <ToastProvider>
        <RouterProvider router={router} />
      </ToastProvider>
    </AuthProvider>
  );
}
```

**Updated AppShell.tsx:**
```typescript
// Add ToastContainer
import ToastContainer from '@/components/ui/ToastContainer';

export default function AppShell() {
  return (
    <div className="min-h-screen bg-metal-dark flex flex-col">
      <Header />
      <main className="flex-1 p-6">
        <Outlet />
      </main>
      <ToastContainer />
    </div>
  );
}
```

---

### B8. Add Global Keyboard Shortcut for Quick Capture
**Depends on:** B7
- [ ] Create useKeyboardShortcut hook
- [ ] Add Ctrl+K / Cmd+K shortcut to open Quick Capture
- [ ] Prevent shortcut when modal is already open

**Files to create:**
- `src/hooks/useKeyboardShortcut.ts`

**Files to modify:**
- `src/components/layout/AppShell.tsx` or `src/pages/Dashboard.tsx`

**useKeyboardShortcut:**
```typescript
// src/hooks/useKeyboardShortcut.ts
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
```

**Usage in Dashboard:**
```typescript
// In Dashboard component
useKeyboardShortcut('k', () => setIsQuickCaptureOpen(true), {
  ctrl: true,
  disabled: isQuickCaptureOpen // Don't trigger when already open
});
```

---

### B9. Write Component Tests
**Depends on:** B7
- [ ] Test QuickCaptureModal opens and closes
- [ ] Test form validation (required fields)
- [ ] Test Enter key submission
- [ ] Test success toast appears after submission
- [ ] Test focus management

**Files to create:**
- `src/components/applications/QuickCaptureModal.test.tsx`

**Test Cases:**
```typescript
describe('QuickCaptureModal', () => {
  it('opens when isOpen is true');
  it('closes when X button is clicked');
  it('closes when Escape key is pressed');
  it('focuses first input when opened');
  it('validates required fields before submit');
  it('shows validation errors inline');
  it('submits form when Enter is pressed');
  it('shows loading state during submission');
  it('shows success toast after submission');
  it('clears form when closed');
});
```

---

## INTEGRATION CHECKLIST (with cadleta)

When cadleta's backend is ready:
- [ ] Update `VITE_API_URL` in `.env` to point to Dev A's backend
- [ ] Test: Open Quick Capture modal from Dashboard
- [ ] Test: Fill all required fields and submit -> 201 Created
- [ ] Test: Success toast shows "+1 POINT!"
- [ ] Test: User's total points update in header
- [ ] Test: Submit with missing required fields -> validation errors
- [ ] Test: Submit without auth token -> 401 redirect to login
- [ ] Test: 15-second completion goal (open -> fill -> submit -> toast)
- [ ] Test: Keyboard shortcut Ctrl+K opens modal
- [ ] Test: Escape key closes modal

---

## COMPLETION CRITERIA

- [ ] Quick Capture modal opens from Dashboard button
- [ ] Modal opens from Ctrl+K / Cmd+K keyboard shortcut
- [ ] Form has company, role, source (required), sourceUrl (optional)
- [ ] Enter key submits the form
- [ ] Validation errors display inline within 100ms
- [ ] Success toast shows "+1 POINT!" message
- [ ] User's points update in header after submission
- [ ] Modal closes on successful submission
- [ ] 15-second completion goal is achievable

---

## GIT WORKFLOW

**Your Branch:** `feature/story2-frontend-quick-capture`

```bash
git checkout main
git pull origin main
git checkout -b feature/story2-frontend-quick-capture
# ... do your work ...
git add .
git commit -m "feat: add Quick Capture modal with toast notifications"
git push -u origin feature/story2-frontend-quick-capture
```

**Commit Convention:** `feat:`, `fix:`, `refactor:`, `docs:`

**Merge Order:** cadleta merges backend first, then you merge frontend.

---

## DEV SERVER

```bash
cd bigjobhunterpro-web
npm run dev
```

Opens at http://localhost:5173

---

## FILE STRUCTURE (New/Modified Files)

```
bigjobhunterpro-web/src/
├── components/
│   ├── applications/
│   │   ├── QuickCaptureModal.tsx (new)
│   │   └── QuickCaptureModal.test.tsx (new)
│   ├── forms/
│   │   └── FormInput.tsx (modified - add forwardRef)
│   ├── layout/
│   │   └── AppShell.tsx (modified - add ToastContainer)
│   └── ui/
│       ├── Modal.tsx (new)
│       ├── Toast.tsx (new)
│       └── ToastContainer.tsx (new)
├── context/
│   ├── AuthContext.tsx (existing)
│   └── ToastContext.tsx (new)
├── hooks/
│   ├── useAuth.ts (existing)
│   └── useKeyboardShortcut.ts (new)
├── services/
│   ├── api.ts (existing)
│   ├── auth.ts (existing)
│   └── applications.ts (new)
├── types/
│   └── application.ts (new)
├── pages/
│   └── Dashboard.tsx (modified - add modal state)
├── App.tsx (modified - add ToastProvider)
└── index.css (modified - add animations)
```
