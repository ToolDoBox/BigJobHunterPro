import { useState } from 'react';
import SidePanel from '../ui/SidePanel';
import type { ContactRelationship, CreateContactRequest } from '../../types/contact';

interface AddContactModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: CreateContactRequest) => void;
}

const RELATIONSHIP_OPTIONS: { value: ContactRelationship; label: string }[] = [
  { value: 'Recruiter', label: 'Recruiter' },
  { value: 'HiringManager', label: 'Hiring Manager' },
  { value: 'TeamMember', label: 'Team Member' },
  { value: 'Other', label: 'Other' },
];

export default function AddContactModal({ isOpen, onClose, onSubmit }: AddContactModalProps) {
  const [name, setName] = useState('');
  const [role, setRole] = useState('');
  const [relationship, setRelationship] = useState<ContactRelationship>('Recruiter');
  const [emailsText, setEmailsText] = useState('');
  const [phonesText, setPhonesText] = useState('');
  const [linkedin, setLinkedin] = useState('');
  const [notes, setNotes] = useState('');
  const [errors, setErrors] = useState<Record<string, string>>({});

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

    // Reset form
    setName('');
    setRole('');
    setRelationship('Recruiter');
    setEmailsText('');
    setPhonesText('');
    setLinkedin('');
    setNotes('');
    setErrors({});
  };

  return (
    <SidePanel isOpen={isOpen} onClose={onClose} title="Add Contact">
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label htmlFor="name" className="block text-sm font-semibold text-gray-300 mb-2">
            Name *
          </label>
          <input
            type="text"
            id="name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className={`input-arcade ${errors.name ? 'input-arcade-error' : ''}`}
            placeholder="e.g., John Smith"
            maxLength={200}
          />
          {errors.name && <p className="text-red text-sm mt-1">{errors.name}</p>}
        </div>

        <div>
          <label htmlFor="role" className="block text-sm font-semibold text-gray-300 mb-2">
            Job Title
          </label>
          <input
            type="text"
            id="role"
            value={role}
            onChange={(e) => setRole(e.target.value)}
            className="input-arcade"
            placeholder="e.g., Senior Technical Recruiter"
            maxLength={100}
          />
        </div>

        <div>
          <label htmlFor="relationship" className="block text-sm font-semibold text-gray-300 mb-2">
            Relationship Type
          </label>
          <select
            id="relationship"
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
          <label htmlFor="emails" className="block text-sm font-semibold text-gray-300 mb-2">
            Email Addresses
          </label>
          <input
            type="text"
            id="emails"
            value={emailsText}
            onChange={(e) => setEmailsText(e.target.value)}
            className="input-arcade"
            placeholder="e.g., john@company.com, john.smith@company.com"
          />
          <p className="text-xs text-gray-400 mt-1">Separate multiple emails with commas</p>
        </div>

        <div>
          <label htmlFor="phones" className="block text-sm font-semibold text-gray-300 mb-2">
            Phone Numbers
          </label>
          <input
            type="text"
            id="phones"
            value={phonesText}
            onChange={(e) => setPhonesText(e.target.value)}
            className="input-arcade"
            placeholder="e.g., 555-123-4567, 555-987-6543"
          />
          <p className="text-xs text-gray-400 mt-1">Separate multiple phones with commas</p>
        </div>

        <div>
          <label htmlFor="linkedin" className="block text-sm font-semibold text-gray-300 mb-2">
            LinkedIn URL
          </label>
          <input
            type="url"
            id="linkedin"
            value={linkedin}
            onChange={(e) => setLinkedin(e.target.value)}
            className="input-arcade"
            placeholder="https://linkedin.com/in/johnsmith"
            maxLength={500}
          />
        </div>

        <div>
          <label htmlFor="notes" className="block text-sm font-semibold text-gray-300 mb-2">
            Notes
          </label>
          <textarea
            id="notes"
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
            Add Contact
          </button>
        </div>
      </form>
    </SidePanel>
  );
}
