import { useState, useEffect, useMemo } from 'react';
import Modal from '../ui/Modal';
import type { Contact } from '@/types/contact';
import type { TimelineEvent } from '@/types/timelineEvent';
import {
  extractFirstName,
  extractTopicFromNotes,
  generateFollowUpEmail,
  buildMailtoUrl,
  openMailtoLink,
} from '@/utils/followUpEmail';
import { useAuth } from '@/hooks/useAuth';

interface FollowUpEmailModalProps {
  isOpen: boolean;
  onClose: () => void;
  companyName: string;
  roleTitle: string;
  contacts: Contact[];
  event: TimelineEvent | null;
}

export default function FollowUpEmailModal({
  isOpen,
  onClose,
  companyName,
  roleTitle,
  contacts,
  event,
}: FollowUpEmailModalProps) {
  const { user } = useAuth();

  // Form state
  const [selectedContactId, setSelectedContactId] = useState<string>('');
  const [manualEmail, setManualEmail] = useState('');
  const [manualName, setManualName] = useState('');
  const [topicDiscussed, setTopicDiscussed] = useState('');
  const [senderName, setSenderName] = useState('');

  // Initialize state when modal opens
  useEffect(() => {
    if (isOpen) {
      // Set sender name from user display name
      setSenderName(user?.displayName || '');

      // Set initial topic from event notes
      const suggestedTopic = extractTopicFromNotes(event?.notes || null);
      setTopicDiscussed(suggestedTopic);

      // Pre-select first contact if available
      if (contacts.length > 0) {
        setSelectedContactId(contacts[0].id);
        setManualEmail('');
        setManualName('');
      } else {
        setSelectedContactId('');
        setManualEmail('');
        setManualName('');
      }
    }
  }, [isOpen, contacts, event, user]);

  // Get the selected contact
  const selectedContact = useMemo(() => {
    if (!selectedContactId) return null;
    return contacts.find(c => c.id === selectedContactId) || null;
  }, [selectedContactId, contacts]);

  // Determine recipient info
  const recipientEmail = selectedContact?.emails[0] || manualEmail.trim();
  const recipientName = selectedContact?.name || manualName.trim();
  const recipientFirstName = extractFirstName(recipientName);

  // Generate the email preview
  const emailPreview = useMemo(() => {
    if (!recipientFirstName || !senderName.trim()) {
      return '';
    }
    return generateFollowUpEmail({
      recipientEmail,
      recipientFirstName,
      senderName: senderName.trim(),
      companyName,
      roleTitle,
      topicDiscussed: topicDiscussed.trim(),
    });
  }, [recipientEmail, recipientFirstName, senderName, companyName, roleTitle, topicDiscussed]);

  const isValid = recipientEmail && recipientFirstName && senderName.trim() && emailPreview;

  const handleOpenEmailClient = () => {
    if (!isValid) return;

    const subject = `Thank you - ${roleTitle} at ${companyName}`;
    const url = buildMailtoUrl(recipientEmail, subject, emailPreview);
    openMailtoLink(url);
    onClose();
  };

  const handleCopyEmail = async () => {
    if (!emailPreview) return;
    try {
      await navigator.clipboard.writeText(emailPreview);
      // Could add a toast here
    } catch {
      // Handle error
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="SEND FOLLOW-UP EMAIL">
      <div className="space-y-4">
        {/* Recipient selection */}
        {contacts.length > 0 ? (
          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Select Contact
            </label>
            <select
              value={selectedContactId}
              onChange={(e) => {
                setSelectedContactId(e.target.value);
                if (e.target.value) {
                  setManualEmail('');
                  setManualName('');
                }
              }}
              className="input-arcade"
            >
              {contacts.map((contact) => (
                <option key={contact.id} value={contact.id}>
                  {contact.name} ({contact.emails[0] || 'no email'})
                </option>
              ))}
              <option value="">-- Enter manually --</option>
            </select>
          </div>
        ) : (
          <p className="text-sm text-gray-400">
            No contacts found. Enter recipient details below.
          </p>
        )}

        {/* Manual entry (show if no contact selected or no contacts) */}
        {(!selectedContactId || contacts.length === 0) && (
          <>
            <div>
              <label className="block text-sm font-semibold text-gray-300 mb-2">
                Recipient Name *
              </label>
              <input
                type="text"
                value={manualName}
                onChange={(e) => setManualName(e.target.value)}
                className="input-arcade"
                placeholder="e.g., John Smith"
              />
            </div>
            <div>
              <label className="block text-sm font-semibold text-gray-300 mb-2">
                Recipient Email *
              </label>
              <input
                type="email"
                value={manualEmail}
                onChange={(e) => setManualEmail(e.target.value)}
                className="input-arcade"
                placeholder="e.g., john@company.com"
              />
            </div>
          </>
        )}

        {/* Your name */}
        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">
            Your Name *
          </label>
          <input
            type="text"
            value={senderName}
            onChange={(e) => setSenderName(e.target.value)}
            className="input-arcade"
            placeholder="Your name for the signature"
          />
        </div>

        {/* Topic discussed */}
        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">
            Topic Discussed (Optional)
          </label>
          <input
            type="text"
            value={topicDiscussed}
            onChange={(e) => setTopicDiscussed(e.target.value)}
            className="input-arcade"
            placeholder="e.g., the team's approach to testing"
          />
          <p className="text-xs text-gray-400 mt-1">
            Leave blank for a generic message
          </p>
        </div>

        {/* Email Preview */}
        {emailPreview && (
          <div>
            <div className="flex items-center justify-between mb-2">
              <label className="block text-sm font-semibold text-gray-300">
                Email Preview
              </label>
              <button
                type="button"
                onClick={handleCopyEmail}
                className="text-xs text-amber hover:text-orange transition-colors"
              >
                Copy
              </button>
            </div>
            <div className="bg-metal-dark/50 border border-metal-light/30 rounded-lg p-4">
              <div className="text-xs text-gray-400 mb-2">
                To: {recipientEmail}
              </div>
              <div className="text-xs text-gray-400 mb-3">
                Subject: Thank you - {roleTitle} at {companyName}
              </div>
              <div className="text-sm text-gray-200 whitespace-pre-wrap">
                {emailPreview}
              </div>
            </div>
          </div>
        )}

        {/* Actions */}
        <div className="flex gap-3 pt-4">
          <button
            type="button"
            onClick={onClose}
            className="btn-metal flex-1"
          >
            CANCEL
          </button>
          <button
            type="button"
            onClick={handleOpenEmailClient}
            disabled={!isValid}
            className="btn-metal-primary flex-1"
          >
            OPEN EMAIL CLIENT
          </button>
        </div>
      </div>
    </Modal>
  );
}
