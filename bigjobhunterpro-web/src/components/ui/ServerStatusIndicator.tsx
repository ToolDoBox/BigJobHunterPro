import { useServerStatus } from '@/hooks/useServerStatus';

type ServerStatus = 'checking' | 'online' | 'offline';

interface ServerStatusIndicatorProps {
    status?: ServerStatus;
}

const statusConfig = {
    checking: {
        label: 'Checking server',
        detail: 'Hold tight while we ping the API.',
        dotClass: 'bg-amber',
        textClass: 'text-amber',
        pulse: true,
    },
    online: {
        label: 'Server online',
        detail: 'Login should respond fast.',
        dotClass: 'bg-terminal',
        textClass: 'text-terminal',
        pulse: false,
    },
    offline: {
        label: 'Server offline',
        detail: 'First login may take ~30s to wake.',
        dotClass: 'bg-led-red',
        textClass: 'text-red-300',
        pulse: true,
    },
};

export default function ServerStatusIndicator({ status: providedStatus }: ServerStatusIndicatorProps) {
    const status = providedStatus ?? useServerStatus();
    const config = statusConfig[status];

    return (
        <div
            className="mt-4 flex flex-col items-center gap-1"
            aria-live="polite"
        >
            <div className="inline-flex items-center gap-2 text-[10px] font-semibold uppercase tracking-[0.2em]">
                <span
                    className={`h-2.5 w-2.5 rounded-full ${config.dotClass} ${config.pulse ? 'animate-pulse motion-reduce:animate-none' : ''}`}
                />
                <span className={config.textClass}>{config.label}</span>
            </div>
            <div className="text-[10px] text-gray-500">{config.detail}</div>
        </div>
    );
}
