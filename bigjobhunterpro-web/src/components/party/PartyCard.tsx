import { useState } from 'react';
import type { HuntingParty } from '@/types/huntingParty';

interface PartyCardProps {
  party: HuntingParty;
  onLeave: () => void;
}

export default function PartyCard({ party, onLeave }: PartyCardProps) {
  const [copied, setCopied] = useState(false);
  const [showLeaveConfirm, setShowLeaveConfirm] = useState(false);

  const handleCopyCode = async () => {
    await navigator.clipboard.writeText(party.inviteCode);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const handleLeaveClick = () => {
    setShowLeaveConfirm(true);
  };

  const handleConfirmLeave = () => {
    setShowLeaveConfirm(false);
    onLeave();
  };

  const handleCancelLeave = () => {
    setShowLeaveConfirm(false);
  };

  return (
    <div className="metal-panel metal-panel-green">
      <div className="metal-panel-screws" />

      <div className="flex items-start justify-between mb-4">
        <div>
          <h3 className="font-arcade text-lg text-terminal mb-1">{party.name}</h3>
          <p className="text-sm text-gray-400">
            {party.memberCount} {party.memberCount === 1 ? 'hunter' : 'hunters'}
          </p>
        </div>
        {party.isCreator && (
          <span className="px-2 py-1 bg-amber/20 text-amber text-xs rounded font-bold">
            CREATOR
          </span>
        )}
      </div>

      <div className="bg-slate-800/50 border border-slate-600 rounded-lg p-3 mb-4">
        <div className="flex items-center justify-between">
          <div>
            <p className="text-xs text-gray-500 mb-1">Invite Code</p>
            <p className="font-arcade text-amber tracking-widest">{party.inviteCode}</p>
          </div>
          <button
            onClick={handleCopyCode}
            className="px-3 py-1 text-sm bg-slate-700 hover:bg-slate-600 text-white rounded transition-colors"
          >
            {copied ? 'Copied!' : 'Copy'}
          </button>
        </div>
      </div>

      {!showLeaveConfirm ? (
        <button
          onClick={handleLeaveClick}
          className="w-full px-4 py-2 text-sm text-red-400 hover:text-red-300 hover:bg-red-900/20 border border-red-800/50 rounded-lg transition-colors"
        >
          Leave Party
        </button>
      ) : (
        <div className="flex gap-2">
          <button
            onClick={handleCancelLeave}
            className="flex-1 px-4 py-2 text-sm bg-slate-700 hover:bg-slate-600 text-white rounded-lg transition-colors"
          >
            Cancel
          </button>
          <button
            onClick={handleConfirmLeave}
            className="flex-1 px-4 py-2 text-sm bg-red-700 hover:bg-red-600 text-white rounded-lg transition-colors"
          >
            Confirm Leave
          </button>
        </div>
      )}
    </div>
  );
}
