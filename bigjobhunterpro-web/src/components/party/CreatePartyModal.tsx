import { useState } from 'react';
import Modal from '@/components/ui/Modal';
import type { HuntingParty } from '@/types/huntingParty';
import { huntingPartiesService } from '@/services/huntingParties';

interface CreatePartyModalProps {
  isOpen: boolean;
  onClose: () => void;
  onCreated: (party: HuntingParty) => void;
}

export default function CreatePartyModal({ isOpen, onClose, onCreated }: CreatePartyModalProps) {
  const [name, setName] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [createdParty, setCreatedParty] = useState<HuntingParty | null>(null);
  const [copied, setCopied] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) {
      setError('Party name is required');
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const party = await huntingPartiesService.createParty({ name: name.trim() });
      setCreatedParty(party);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create party');
    } finally {
      setIsLoading(false);
    }
  };

  const handleCopyCode = async () => {
    if (createdParty) {
      await navigator.clipboard.writeText(createdParty.inviteCode);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    }
  };

  const handleDone = () => {
    if (createdParty) {
      onCreated(createdParty);
    }
    handleClose();
  };

  const handleClose = () => {
    setName('');
    setError(null);
    setCreatedParty(null);
    setCopied(false);
    onClose();
  };

  return (
    <Modal isOpen={isOpen} onClose={handleClose} title="Create Hunting Party">
      {!createdParty ? (
        <form onSubmit={handleSubmit}>
          <div className="mb-4">
            <label htmlFor="partyName" className="block text-sm text-gray-300 mb-2">
              Party Name
            </label>
            <input
              id="partyName"
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="e.g., The Job Hunters"
              maxLength={100}
              className="w-full px-4 py-3 bg-slate-800/50 border border-slate-600 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-blaze focus:ring-1 focus:ring-blaze"
              autoFocus
              disabled={isLoading}
            />
          </div>

          {error && (
            <div className="mb-4 p-3 bg-red-900/30 border border-red-700 rounded-lg text-red-400 text-sm">
              {error}
            </div>
          )}

          <div className="flex gap-3">
            <button
              type="button"
              onClick={handleClose}
              className="flex-1 px-4 py-3 bg-slate-700 hover:bg-slate-600 text-white rounded-lg transition-colors"
              disabled={isLoading}
            >
              Cancel
            </button>
            <button
              type="submit"
              className="flex-1 px-4 py-3 bg-blaze hover:bg-orange-600 text-white font-bold rounded-lg transition-colors disabled:opacity-50"
              disabled={isLoading || !name.trim()}
            >
              {isLoading ? 'Creating...' : 'Create Party'}
            </button>
          </div>
        </form>
      ) : (
        <div className="text-center">
          <div className="mb-4">
            <div className="text-terminal text-xl mb-2">Party Created!</div>
            <p className="text-gray-300">Share this invite code with your friends:</p>
          </div>

          <div className="bg-slate-800/50 border border-slate-600 rounded-lg p-4 mb-4">
            <div className="font-arcade text-2xl text-amber tracking-widest mb-2">
              {createdParty.inviteCode}
            </div>
            <button
              onClick={handleCopyCode}
              className="text-sm text-blaze hover:text-orange-400 transition-colors"
            >
              {copied ? 'Copied!' : 'Copy to clipboard'}
            </button>
          </div>

          <button
            onClick={handleDone}
            className="w-full px-4 py-3 bg-blaze hover:bg-orange-600 text-white font-bold rounded-lg transition-colors"
          >
            Let's Hunt!
          </button>
        </div>
      )}
    </Modal>
  );
}
