import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import ApplicationDetail from '@/pages/ApplicationDetail';

const getApplicationMock = vi.fn();
const updateApplicationMock = vi.fn();
const deleteApplicationMock = vi.fn();
const showToastMock = vi.fn();
const refreshUserMock = vi.fn();

vi.mock('@/services/applications', () => ({
  default: {
    getApplication: (...args: unknown[]) => getApplicationMock(...args),
    updateApplication: (...args: unknown[]) => updateApplicationMock(...args),
    deleteApplication: (...args: unknown[]) => deleteApplicationMock(...args),
  },
}));

vi.mock('@/context/ToastContext', () => ({
  useToast: () => ({
    showToast: showToastMock,
  }),
}));

vi.mock('@/hooks/useAuth', () => ({
  useAuth: () => ({
    refreshUser: refreshUserMock,
  }),
}));

const baseApplication = {
  id: 'app-123',
  companyName: 'Acme Corp',
  roleTitle: 'Engineer',
  sourceName: 'LinkedIn',
  sourceUrl: 'https://linkedin.com/jobs/123',
  status: 'Applied',
  workMode: 'Remote',
  location: 'Dallas, TX',
  salaryMin: 120,
  salaryMax: 150,
  jobDescription: 'Build systems.',
  requiredSkills: ['C#'],
  niceToHaveSkills: ['Azure'],
  parsedByAI: false,
  aiParsingStatus: 'Pending',
  points: 1,
  createdDate: new Date().toISOString(),
  updatedDate: new Date().toISOString(),
  lastAIParsedDate: null,
  rawPageContent: 'Sample job content',
};

describe('ApplicationDetail', () => {
  beforeEach(() => {
    getApplicationMock.mockReset();
    updateApplicationMock.mockReset();
    deleteApplicationMock.mockReset();
    showToastMock.mockReset();
    refreshUserMock.mockReset();
  });

  it('renders application details and saves edits', async () => {
    const user = userEvent.setup();
    getApplicationMock.mockResolvedValue(baseApplication);
    updateApplicationMock.mockResolvedValue({
      ...baseApplication,
      companyName: 'Nimbus Labs',
    });

    render(
      <MemoryRouter initialEntries={['/app/applications/app-123']}>
        <Routes>
          <Route path="/app/applications/:id" element={<ApplicationDetail />} />
        </Routes>
      </MemoryRouter>
    );

    expect(await screen.findByText('Acme Corp')).toBeInTheDocument();

    await user.click(screen.getByRole('button', { name: /edit/i }));

    const companyInput = screen.getByLabelText('COMPANY');
    await user.clear(companyInput);
    await user.type(companyInput, 'Nimbus Labs');

    await user.click(screen.getByRole('button', { name: /save changes/i }));

    await waitFor(() => {
      expect(updateApplicationMock).toHaveBeenCalledTimes(1);
    });

    expect(showToastMock).toHaveBeenCalledWith(
      'success',
      'Application updated successfully!'
    );
  });
});
