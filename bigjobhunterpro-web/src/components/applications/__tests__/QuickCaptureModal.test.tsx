import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import QuickCaptureModal from '@/components/applications/QuickCaptureModal';

const createApplicationMock = vi.fn();
const refreshUserMock = vi.fn();
const showToastMock = vi.fn();

vi.mock('@/services/applications', () => ({
  default: {
    createApplication: (...args: unknown[]) => createApplicationMock(...args),
  },
}));

vi.mock('@/hooks/useAuth', () => ({
  useAuth: () => ({
    refreshUser: refreshUserMock,
  }),
}));

vi.mock('@/context/ToastContext', () => ({
  useToast: () => ({
    showToast: showToastMock,
  }),
}));

describe('QuickCaptureModal', () => {
  beforeEach(() => {
    createApplicationMock.mockReset();
    refreshUserMock.mockReset();
    showToastMock.mockReset();
  });

  it('shows validation errors when required fields are missing', async () => {
    const user = userEvent.setup();
    render(
      <QuickCaptureModal isOpen={true} onClose={vi.fn()} />
    );

    await user.click(screen.getByRole('button', { name: /lock it in/i }));

    expect(screen.getByText('Job page content is required')).toBeInTheDocument();
  });

  it('submits valid data and shows success feedback', async () => {
    const user = userEvent.setup();
    const onClose = vi.fn();
    const onSuccess = vi.fn();

    createApplicationMock.mockResolvedValue({
      id: '123',
      sourceUrl: null,
      status: 'Applied',
      aiParsingStatus: 'Pending',
      points: 1,
      totalPoints: 1,
      createdDate: new Date().toISOString(),
    });

    render(
      <QuickCaptureModal isOpen={true} onClose={onClose} onSuccess={onSuccess} />
    );

    await user.type(screen.getByLabelText('JOB URL'), 'https://example.com/job/123');
    await user.type(screen.getByLabelText('JOB PAGE CONTENT'), 'Sample job page content');
    await user.click(screen.getByRole('button', { name: /lock it in/i }));

    await waitFor(() => {
      expect(createApplicationMock).toHaveBeenCalledTimes(1);
    });

    expect(refreshUserMock).toHaveBeenCalledTimes(1);
    expect(showToastMock).toHaveBeenCalledWith(
      'success',
      'Hunt logged successfully!',
      1
    );
    expect(onClose).toHaveBeenCalledTimes(1);
    expect(onSuccess).toHaveBeenCalledTimes(1);
  });
});
