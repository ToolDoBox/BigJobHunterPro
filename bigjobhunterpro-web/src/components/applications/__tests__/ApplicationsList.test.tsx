import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import ApplicationsList from '@/components/applications/ApplicationsList';
import type { ApplicationListItem } from '@/types/application';

describe('ApplicationsList', () => {
  it('renders application rows with key details', () => {
    const items: ApplicationListItem[] = [
      {
        id: 'app-1',
        companyName: 'Acme Corp',
        roleTitle: 'Design Engineer',
        status: 'Applied',
        createdDate: new Date().toISOString(),
      },
    ];

    render(
      <MemoryRouter>
        <ApplicationsList items={items} />
      </MemoryRouter>
    );

    expect(screen.getByText('COMPANY')).toBeInTheDocument();
    expect(screen.getByText('ROLE')).toBeInTheDocument();
    expect(screen.getByText('STATUS')).toBeInTheDocument();
    expect(screen.getByText('DATE APPLIED')).toBeInTheDocument();
    expect(screen.getAllByText('Acme Corp')).toHaveLength(2);
    expect(screen.getAllByText('Design Engineer')).toHaveLength(2);
    expect(screen.getAllByText('APPLIED')).not.toHaveLength(0);
  });
});
