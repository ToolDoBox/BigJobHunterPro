import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import { HubConnectionState } from '@microsoft/signalr';
import ActivityFeed from '@/components/party/ActivityFeed';
import type { ActivityEvent } from '@/types/activity';

describe('ActivityFeed', () => {
  it('renders activity events with labels and points', () => {
    const events: ActivityEvent[] = [
      {
        id: 'event-1',
        partyId: 'party-1',
        userId: 'user-1',
        userDisplayName: 'Alex Hunter',
        eventType: 'ApplicationLogged',
        pointsDelta: 1,
        createdDate: new Date().toISOString(),
        companyName: 'Acme Corp',
        roleTitle: 'Engineer',
        milestoneLabel: null,
      },
    ];

    render(
      <ActivityFeed
        events={events}
        isLoading={false}
        connectionState={HubConnectionState.Connected}
      />
    );

    expect(screen.getByText('Alex Hunter')).toBeInTheDocument();
    expect(screen.getByText('logged an application')).toBeInTheDocument();
    expect(screen.getByText('+1 pts')).toBeInTheDocument();
  });
});
