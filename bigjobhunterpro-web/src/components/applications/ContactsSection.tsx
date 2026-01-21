import { useState } from 'react';
import { contactsService } from '@/services/contacts';
import { useToast } from '@/context/ToastContext';
import AddContactModal from './AddContactModal';
import EditContactModal from './EditContactModal';
import type { Contact, CreateContactRequest, UpdateContactRequest } from '@/types/contact';

interface ContactsSectionProps {
  applicationId: string;
  contacts: Contact[];
  onUpdated: () => void;
}

const RELATIONSHIP_LABELS: Record<string, string> = {
  Recruiter: 'Recruiter',
  HiringManager: 'Hiring Manager',
  TeamMember: 'Team Member',
  Other: 'Other',
};

export default function ContactsSection({
  applicationId,
  contacts,
  onUpdated,
}: ContactsSectionProps) {
  const { showToast } = useToast();
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [editingContact, setEditingContact] = useState<Contact | null>(null);
  const [isDeleting, setIsDeleting] = useState<string | null>(null);

  const handleAddContact = async (data: CreateContactRequest) => {
    try {
      await contactsService.create(applicationId, data);
      showToast('success', 'Contact added successfully');
      setIsAddModalOpen(false);
      onUpdated();
    } catch (error) {
      showToast('error', error instanceof Error ? error.message : 'Failed to add contact');
    }
  };

  const handleEditClick = (contact: Contact) => {
    setEditingContact(contact);
    setIsEditModalOpen(true);
  };

  const handleUpdateContact = async (data: UpdateContactRequest) => {
    if (!editingContact) return;

    try {
      await contactsService.update(applicationId, editingContact.id, data);
      showToast('success', 'Contact updated successfully');
      setIsEditModalOpen(false);
      setEditingContact(null);
      onUpdated();
    } catch (error) {
      showToast('error', error instanceof Error ? error.message : 'Failed to update contact');
    }
  };

  const handleDeleteContact = async (contactId: string) => {
    if (!confirm('Are you sure you want to delete this contact?')) return;

    setIsDeleting(contactId);
    try {
      await contactsService.delete(applicationId, contactId);
      showToast('success', 'Contact deleted successfully');
      onUpdated();
    } catch (error) {
      showToast('error', error instanceof Error ? error.message : 'Failed to delete contact');
    } finally {
      setIsDeleting(null);
    }
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <h3 className="font-arcade text-xs text-amber">CONTACTS</h3>
        <button
          className="btn-metal text-xs"
          onClick={() => setIsAddModalOpen(true)}
        >
          + ADD CONTACT
        </button>
      </div>

      {contacts.length === 0 ? (
        <div className="text-center py-6 bg-metal-dark/30 rounded-lg border border-metal-light/20">
          <p className="text-gray-400 text-sm">
            No contacts added yet. Add recruiters, hiring managers, or other contacts associated with this application.
          </p>
        </div>
      ) : (
        <div className="space-y-3">
          {contacts.map((contact) => (
            <div
              key={contact.id}
              className="bg-metal-dark/30 rounded-lg border border-metal-light/20 p-4"
            >
              <div className="flex justify-between items-start">
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2 mb-1">
                    <span className="font-semibold text-white">{contact.name}</span>
                    <span className="text-xs px-2 py-0.5 bg-metal-light/20 rounded text-gray-300">
                      {RELATIONSHIP_LABELS[contact.relationship] || contact.relationship}
                    </span>
                  </div>
                  {contact.role && (
                    <p className="text-sm text-gray-400 mb-2">{contact.role}</p>
                  )}

                  <div className="space-y-1 text-sm">
                    {contact.emails.length > 0 && (
                      <div className="flex items-start gap-2">
                        <span className="text-gray-500 w-14 flex-shrink-0">Email:</span>
                        <div className="flex flex-wrap gap-x-3 gap-y-1">
                          {contact.emails.map((email, idx) => (
                            <a
                              key={idx}
                              href={`mailto:${email}`}
                              className="text-amber hover:text-orange transition-colors"
                            >
                              {email}
                            </a>
                          ))}
                        </div>
                      </div>
                    )}
                    {contact.phones.length > 0 && (
                      <div className="flex items-start gap-2">
                        <span className="text-gray-500 w-14 flex-shrink-0">Phone:</span>
                        <div className="flex flex-wrap gap-x-3 gap-y-1">
                          {contact.phones.map((phone, idx) => (
                            <a
                              key={idx}
                              href={`tel:${phone}`}
                              className="text-amber hover:text-orange transition-colors"
                            >
                              {phone}
                            </a>
                          ))}
                        </div>
                      </div>
                    )}
                    {contact.linkedin && (
                      <div className="flex items-start gap-2">
                        <span className="text-gray-500 w-14 flex-shrink-0">LinkedIn:</span>
                        <a
                          href={contact.linkedin}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="text-amber hover:text-orange transition-colors truncate"
                        >
                          {contact.linkedin}
                        </a>
                      </div>
                    )}
                  </div>

                  {contact.notes && (
                    <p className="text-sm text-gray-400 mt-2 whitespace-pre-wrap">{contact.notes}</p>
                  )}
                </div>

                <div className="flex gap-2 ml-3 flex-shrink-0">
                  <button
                    className="text-gray-400 hover:text-amber transition-colors text-sm"
                    onClick={() => handleEditClick(contact)}
                    title="Edit contact"
                  >
                    Edit
                  </button>
                  <button
                    className="text-gray-400 hover:text-red transition-colors text-sm"
                    onClick={() => handleDeleteContact(contact.id)}
                    disabled={isDeleting === contact.id}
                    title="Delete contact"
                  >
                    {isDeleting === contact.id ? '...' : 'Delete'}
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      <AddContactModal
        isOpen={isAddModalOpen}
        onClose={() => setIsAddModalOpen(false)}
        onSubmit={handleAddContact}
      />

      <EditContactModal
        isOpen={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setEditingContact(null);
        }}
        onSubmit={handleUpdateContact}
        contact={editingContact}
      />
    </div>
  );
}
