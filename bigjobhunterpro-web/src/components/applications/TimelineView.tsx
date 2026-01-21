import { useState } from 'react';
import type { TimelineEvent } from '../../types/timelineEvent';
import type { Contact } from '../../types/contact';
import { EventTypeBadge } from './EventTypeBadge';
import { AddTimelineEventModal } from './AddTimelineEventModal';
import { EditTimelineEventModal } from './EditTimelineEventModal';
import FollowUpEmailModal from './FollowUpEmailModal';
import { timelineEventsService } from '../../services/timelineEvents';
import { useToast } from '../../context/ToastContext';
import { useAuth } from '../../hooks/useAuth';

interface TimelineViewProps {
  applicationId: string;
  events: TimelineEvent[];
  onEventsUpdated: () => void;
  companyName: string;
  roleTitle: string;
  contacts: Contact[];
}

export function TimelineView({
  applicationId,
  events,
  onEventsUpdated,
  companyName,
  roleTitle,
  contacts,
}: TimelineViewProps) {
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [editingEvent, setEditingEvent] = useState<TimelineEvent | null>(null);
  const [isDeleting, setIsDeleting] = useState<string | null>(null);
  const [followUpEvent, setFollowUpEvent] = useState<TimelineEvent | null>(null);
  const { showToast } = useToast();

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: 'numeric',
      minute: '2-digit',
    });
  };

  const { refreshUser } = useAuth();

  const handleAddEvent = async (data: any) => {
    try {
      await timelineEventsService.create(applicationId, data);
      showToast('success', 'Timeline event added successfully!', data.eventType === 'Interview' ? 5 : undefined);
      await refreshUser(); // Update user points
      setIsAddModalOpen(false);
      onEventsUpdated();
    } catch (error: any) {
      showToast('error', error instanceof Error ? error.message : 'Unable to add the timeline event.');
    }
  };

  const handleEditEvent = async (data: any) => {
    if (!editingEvent) return;

    try {
      await timelineEventsService.update(applicationId, editingEvent.id, data);
      showToast('success', 'Event updated successfully');
      await refreshUser(); // Update user points
      setEditingEvent(null);
      onEventsUpdated();
    } catch (error: any) {
      showToast('error', error instanceof Error ? error.message : 'Unable to update the timeline event.');
    }
  };

  const handleDeleteEvent = async (eventId: string) => {
    if (!confirm('Are you sure you want to delete this event? Points will be recalculated.')) {
      return;
    }

    setIsDeleting(eventId);
    try {
      await timelineEventsService.delete(applicationId, eventId);
      showToast('success', 'Event deleted successfully');
      await refreshUser(); // Update user points
      onEventsUpdated();
    } catch (error: any) {
      showToast('error', error instanceof Error ? error.message : 'Unable to delete the timeline event.');
    } finally {
      setIsDeleting(null);
    }
  };

  const totalPoints = events.reduce((sum, event) => sum + event.points, 0);

  return (
    <div>
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-lg font-arcade text-terminal">TIMELINE</h3>
        <button
          type="button"
          onClick={() => setIsAddModalOpen(true)}
          className="btn-metal-primary text-sm"
        >
          + Add Event
        </button>
      </div>

      <div className="bg-metal-dark p-4 rounded border border-gray-700 mb-4">
        <div className="text-sm text-gray-400">Total Points from Events:</div>
        <div className="text-2xl font-arcade text-terminal">{totalPoints} PTS</div>
      </div>

      <div className="space-y-3">
        {events.length === 0 ? (
          <div className="text-center py-8 text-gray-400">
            <p>No timeline events yet.</p>
            <p className="text-sm mt-2">Add your first event to start tracking!</p>
          </div>
        ) : (
          events.map((event) => (
            <div
              key={event.id}
              className="metal-panel p-4 hover:border-blaze transition-colors"
            >
              <div className="flex items-start justify-between">
                <div className="flex-1">
                  <div className="flex items-center gap-3 mb-2">
                    <EventTypeBadge
                      eventType={event.eventType}
                      interviewRound={event.interviewRound}
                    />
                    <span className="text-xs text-gray-400">
                      {formatDate(event.timestamp)}
                    </span>
                    <span className="text-xs font-semibold text-terminal">
                      +{event.points} pts
                    </span>
                  </div>

                  {event.notes && (
                    <p className="text-sm text-gray-300 mt-2">{event.notes}</p>
                  )}
                </div>

                <div className="flex items-center gap-2 ml-4">
                  {/* Follow-up email button for Interview and Screening events */}
                  {(event.eventType === 'Interview' || event.eventType === 'Screening') && (
                    <button
                      onClick={() => setFollowUpEvent(event)}
                      className="text-amber hover:text-orange text-sm"
                      aria-label="Generate follow-up email"
                      title="Generate follow-up email"
                    >
                      📧
                    </button>
                  )}
                  <button
                    onClick={() => setEditingEvent(event)}
                    className="text-terminal hover:text-crt-amber text-sm"
                    aria-label="Edit event"
                    title="Edit event"
                  >
                    ✏️
                  </button>
                  <button
                    onClick={() => handleDeleteEvent(event.id)}
                    disabled={isDeleting === event.id}
                    className="text-red hover:text-red-400 text-sm"
                    aria-label="Delete event"
                    title="Delete event"
                  >
                    {isDeleting === event.id ? 'Deleting...' : '✕'}
                  </button>
                </div>
              </div>
            </div>
          ))
        )}
      </div>

      <AddTimelineEventModal
        isOpen={isAddModalOpen}
        onClose={() => setIsAddModalOpen(false)}
        onSubmit={handleAddEvent}
      />

      <EditTimelineEventModal
        isOpen={!!editingEvent}
        onClose={() => setEditingEvent(null)}
        onSubmit={handleEditEvent}
        event={editingEvent}
      />

      <FollowUpEmailModal
        isOpen={!!followUpEvent}
        onClose={() => setFollowUpEvent(null)}
        companyName={companyName}
        roleTitle={roleTitle}
        contacts={contacts}
        event={followUpEvent}
      />
    </div>
  );
}
