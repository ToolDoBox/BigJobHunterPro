import { useEffect, useState } from 'react';
import api from '@/services/api';

type ServerStatus = 'checking' | 'online' | 'offline';

const CHECK_INTERVAL_MS = 30000;
const REQUEST_TIMEOUT_MS = 2500;

export function useServerStatus() {
    const [status, setStatus] = useState<ServerStatus>('checking');

    useEffect(() => {
        let isMounted = true;

        const checkStatus = async () => {
            try {
                await api.get('/api/health', { timeout: REQUEST_TIMEOUT_MS });
                if (isMounted) {
                    setStatus('online');
                }
            } catch {
                if (isMounted) {
                    setStatus('offline');
                }
            }
        };

        checkStatus();
        const intervalId = window.setInterval(checkStatus, CHECK_INTERVAL_MS);

        return () => {
            isMounted = false;
            window.clearInterval(intervalId);
        };
    }, []);

    return status;
}
