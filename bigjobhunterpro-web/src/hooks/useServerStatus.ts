import { useEffect, useState } from 'react';
import api from '@/services/api';

type ServerStatus = 'checking' | 'online' | 'offline';

const CHECK_INTERVAL_MS = 30000; // 30 seconds
const REQUEST_TIMEOUT_MS = 5000; // 5 seconds (increased for comprehensive check)

/**
 * Hook to monitor backend server health status.
 *
 * Uses the comprehensive /api/health/ready endpoint which validates:
 * - Database connectivity
 * - JWT configuration
 * - Database migrations
 * - Critical configuration
 *
 * Returns 'online' only when all critical components are healthy.
 */
export function useServerStatus() {
    const [status, setStatus] = useState<ServerStatus>('checking');

    useEffect(() => {
        let isMounted = true;

        const checkStatus = async () => {
            try {
                // Use the comprehensive readiness endpoint
                // This returns 200 OK only when all critical systems are healthy
                // Returns 503 if degraded or unhealthy
                const response = await api.get('/api/health/ready', {
                    timeout: REQUEST_TIMEOUT_MS
                });

                if (isMounted) {
                    // 200 OK means system is fully ready
                    setStatus('online');
                }
            } catch (error: any) {
                if (isMounted) {
                    // Any error (503, timeout, network error) means offline
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
