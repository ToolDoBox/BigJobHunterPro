import { useState, useEffect } from 'react';
import SidePanel from '../ui/SidePanel';
import type { Contact, ContactRelationship, UpdateContactRequest } from '../../types/contact';

interface EditContactModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: UpdateContactRequest) => void;
  contact: Contact | null;
}

const RELATIONSHIP_OPTIONS: { value: ContactRelationship; label: string }[] = [
  { value: 'Recruiter', label: 'Recruiter' },
  { value: 'HiringManager', label: 'Hiring Manager' },
  { value: 'TeamMember', label: 'Team Member' },
  { value: 'Other', label: 'Other' },
];

export default function EditContactModal({ isOpen, onClose, onSubmit, contact }: EditContactModalProps) {
  const [name, setName] = useState('');
  const [role, setRole] = useState('');
  const [relationship, setRelationship] = useState<ContactRelationship>('Recruiter');
  const [emailsText, setEmailsText] = useState('');
  const [phonesText, setPhonesText] = useState('');
  const [linkedin, setLinkedin] = useState('');
  const [notes, setNotes] = useState('');
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (contact) {
      setName(contact.name);
      setRole(contact.role || '');
      setRelationship(contact.relationship);
      setEmailsText(contact.emails.join(', '));
      setPhonesText(contact.phones.join(', '));
      setLinkedin(contact.linkedin || '');
      setNotes(contact.notes || '');
      setErrors({});
    }
  }, [contact]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const newErrors: Record<string, string> = {};
    if (!name.trim()) newErrors.name = 'Name is required';

    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }

    // Parse emails and phones from comma-separated text
    const emails = emailsText
      .split(',')
      .map(e => e.trim())
      .filter(e => e.length > 0);
    const phones = phonesText
      .split(',')
      .map(p => p.trim())
      .filter(p => p.length > 0);

    onSubmit({
      name: name.trim(),
      role: role.trim() || undefined,
      relationship,
      emails,
      phones,
      linkedin: linkedin.trim() || undefined,
      notes: notes.trim() || undefined,
    });
  };

  if (!contact) return null;

  return (
    <SidePanel isOpen={isOpen} onClose={onClose} title="Edit Contact">
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label htmlFor="edit-name" className="block text-sm font-semibold text-gray-300 mb-2">
            Name *
          </label>
          <input
            type="text"
            id="edit-name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className={`input-arcade ${errors.name ? 'input-arcade-error' : ''}`}
            placeholder="e.g., John Smith"
            maxLength={200}
          />
          {errors.name && <p className="text-red text-sm mt-1">{errors.name}</p>}
        </div>

        <div>
          <label htmlFor="edit-role" className="block text-sm font-semibold text-gray-300 mb-2">
            Job Title
          </label>
          <input
            type="text"
            id="edit-role"
            value={role}
            onChange={(e) => setRole(e.target.value)}
            className="input-arcade"
            placeholder="e.g., Senior Technical Recruiter"
            maxLength={100}
          />
        </div>

        <div>
          <label htmlFor="edit-relationship" className="block text-sm font-semibold text-gray-300 mb-2">
            Relationship Type
          </label>
          <select
            id="edit-relationship"
            value={relationship}
            onChange={(e) => setRelationship(e.target.value as ContactRelationship)}
            className="input-arcade"
          >
            {RELATIONSHIP_OPTIONS.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label htmlFor="edit-emails" className="block text-sm font-semibold text-gray-300 mb-2">
            Email Addresses
          </label>
          <input
            type="text"
            id="edit-emails"
            value={emailsText}
            onChange={(e) => setEmailsText(e.target.value)}
            className="input-arcade"
            placeholder="e.g., john@company.com, john.smith@company.com"
          />
          <p className="text-xs text-gray-400 mt-1">Separate multiple emails with commas</p>
        </div>

        <div>
          <label htmlFor="edit-phones" className="block text-sm font-semibold text-gray-300 mb-2">
            Phone Numbers
          </label>
          <input
            type="text"
            id="edit-phones"
            value={phonesText}
            onChange={(e) => setPhonesText(e.target.value)}
            className="input-arcade"
            placeholder="e.g., 555-123-4567, 555-987-6543"
          />
          <p className="text-xs text-gray-400 mt-1">Separate multiple phones with commas</p>
        </div>

        <div>
          <label htmlFor="edit-linkedin" className="block text-sm font-semibold text-gray-300 mb-2">
            LinkedIn URL
          </label>
          <input
            type="url"
            id="edit-linkedin"
            value={linkedin}
            onChange={(e) => setLinkedin(e.target.value)}
            className="input-arcade"
            placeholder="https://linkedin.com/in/johnsmith"
            maxLength={500}
          />
        </div>

        <div>
          <label htmlFor="edit-notes" className="block text-sm font-semibold text-gray-300 mb-2">
            Notes
          </label>
          <textarea
            id="edit-notes"
            rows={4}
            maxLength={2000}
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            className="input-arcade"
            placeholder="Any additional notes about this contact..."
          />
          <div className="text-xs text-gray-400 mt-1">{notes.length}/2000 characters</div>
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
            Save Changes
          </button>
        </div>
      </form>
    </SidePanel>
  );
}
