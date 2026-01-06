import { useState } from 'react';
import Modal from '@/components/ui/Modal';
import type { HuntingParty } from '@/types/huntingParty';
import { huntingPartiesService } from '@/services/huntingParties';

interface JoinPartyModalProps {
  isOpen: boolean;
  onClose: () => void;
  onJoined: (party: HuntingParty) => void;
}

export default function JoinPartyModal({ isOpen, onClose, onJoined }: JoinPartyModalProps) {
  const [inviteCode, setInviteCode] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!inviteCode.trim()) {
      setError('Invite code is required');
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const party = await huntingPartiesService.joinParty({ inviteCode: inviteCode.trim().toUpperCase() });
      onJoined(party);
      handleClose();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to join party');
    } finally {
      setIsLoading(false);
    }
  };

  const handleClose = () => {
    setInviteCode('');
    setError(null);
    onClose();
  };

  const handleCodeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    // Only allow alphanumeric characters and convert to uppercase
    const value = e.target.value.replace(/[^A-Za-z0-9]/g, '').toUpperCase();
    setInviteCode(value);
  };

  return (
    <Modal isOpen={isOpen} onClose={handleClose} title="Join Hunting Party">
      <form onSubmit={handleSubmit}>
        <div className="mb-4">
          <label htmlFor="inviteCode" className="block text-sm text-gray-300 mb-2">
            Invite Code
          </label>
          <input
            id="inviteCode"
            type="text"
            value={inviteCode}
            onChange={handleCodeChange}
            placeholder="Enter 8-character code"
            maxLength={8}
            className="w-full px-4 py-3 bg-slate-800/50 border border-slate-600 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-blaze focus:ring-1 focus:ring-blaze font-arcade text-center tracking-widest text-lg"
            autoFocus
            disabled={isLoading}
          />
          <p className="mt-2 text-xs text-gray-500">
            Get an invite code from a friend who created a party
          </p>
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
            disabled={isLoading || inviteCode.length !== 8}
          >
            {isLoading ? 'Joining...' : 'Join Party'}
          </button>
        </div>
      </form>
    </Modal>
  );
}
