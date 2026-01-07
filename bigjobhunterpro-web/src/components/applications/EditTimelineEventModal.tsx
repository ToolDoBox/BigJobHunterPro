import { useState, useEffect } from 'react';
import Modal from '../ui/Modal';
import type { EventType, TimelineEvent } from '../../types/timelineEvent';

interface EditTimelineEventModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: {
    eventType: EventType;
    interviewRound?: number;
    timestamp: string;
    notes?: string;
  }) => void;
  event: TimelineEvent | null;
}

const EVENT_TYPE_OPTIONS: { value: EventType; label: string; points: number }[] = [
  { value: 'Prospecting', label: 'Prospecting', points: 0 },
  { value: 'Applied', label: 'Applied', points: 1 },
  { value: 'Screening', label: 'Screening', points: 2 },
  { value: 'Interview', label: 'Interview', points: 5 },
  { value: 'Offer', label: 'Offer', points: 50 },
  { value: 'Rejected', label: 'Rejected', points: 2 },
  { value: 'Withdrawn', label: 'Withdrawn', points: 0 },
];

export function EditTimelineEventModal({ isOpen, onClose, onSubmit, event }: EditTimelineEventModalProps) {
  const [eventType, setEventType] = useState<EventType>('Applied');
  const [interviewRound, setInterviewRound] = useState<number>(1);
  const [timestamp, setTimestamp] = useState<string>('');
  const [notes, setNotes] = useState<string>('');
  const [errors, setErrors] = useState<Record<string, string>>({});

  // Pre-fill form when event changes
  useEffect(() => {
    if (event) {
      setEventType(event.eventType);
      setInterviewRound(event.interviewRound || 1);
      // Convert ISO timestamp to datetime-local format
      setTimestamp(new Date(event.timestamp).toISOString().slice(0, 16));
      setNotes(event.notes || '');
    }
  }, [event]);

  const selectedOption = EVENT_TYPE_OPTIONS.find(opt => opt.value === eventType);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    // Basic validation
    const newErrors: Record<string, string> = {};
    if (!eventType) newErrors.eventType = 'Event type is required';
    if (!timestamp) newErrors.timestamp = 'Timestamp is required';
    if (eventType === 'Interview' && !interviewRound) {
      newErrors.interviewRound = 'Interview round is required';
    }

    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }

    onSubmit({
      eventType,
      interviewRound: eventType === 'Interview' ? interviewRound : undefined,
      timestamp: new Date(timestamp).toISOString(),
      notes: notes.trim() || undefined,
    });

    setErrors({});
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Edit Timeline Event">
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label htmlFor="editEventType" className="block text-sm font-semibold text-gray-300 mb-2">
            Event Type
          </label>
          <select
            id="editEventType"
            value={eventType}
            onChange={(e) => setEventType(e.target.value as EventType)}
            className={`input-arcade ${errors.eventType ? 'input-arcade-error' : ''}`}
          >
            {EVENT_TYPE_OPTIONS.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
          {errors.eventType && <p className="text-red text-sm mt-1">{errors.eventType}</p>}
        </div>

        {eventType === 'Interview' && (
          <div>
            <label htmlFor="editInterviewRound" className="block text-sm font-semibold text-gray-300 mb-2">
              Interview Round
            </label>
            <input
              type="number"
              id="editInterviewRound"
              min="1"
              value={interviewRound}
              onChange={(e) => setInterviewRound(parseInt(e.target.value, 10))}
              className={`input-arcade ${errors.interviewRound ? 'input-arcade-error' : ''}`}
            />
            {errors.interviewRound && <p className="text-red text-sm mt-1">{errors.interviewRound}</p>}
          </div>
        )}

        <div>
          <label htmlFor="editTimestamp" className="block text-sm font-semibold text-gray-300 mb-2">
            Date & Time
          </label>
          <input
            type="datetime-local"
            id="editTimestamp"
            value={timestamp}
            onChange={(e) => setTimestamp(e.target.value)}
            className={`input-arcade ${errors.timestamp ? 'input-arcade-error' : ''}`}
          />
          {errors.timestamp && <p className="text-red text-sm mt-1">{errors.timestamp}</p>}
        </div>

        <div>
          <label htmlFor="editNotes" className="block text-sm font-semibold text-gray-300 mb-2">
            Notes (Optional)
          </label>
          <textarea
            id="editNotes"
            rows={3}
            maxLength={1000}
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            className="input-arcade"
            placeholder="Add any notes about this event..."
          />
          <div className="text-xs text-gray-400 mt-1">{notes.length}/1000 characters</div>
        </div>

        <div className="bg-metal-dark p-3 rounded border border-gray-700">
          <div className="text-sm text-gray-400">Points for this event:</div>
          <div className="text-2xl font-arcade text-terminal mt-1">+{selectedOption?.points} PTS</div>
        </div>

        <div className="flex gap-3 justify-end pt-4">
          <button
            type="button"
            onClick={onClose}
            className="btn-metal"
          >
            Cancel
          </button>
          <button type="submit" className="btn-metal-primary">
            Update Event
          </button>
        </div>
      </form>
    </Modal>
  );
}
