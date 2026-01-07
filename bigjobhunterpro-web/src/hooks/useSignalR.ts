import { useEffect, useRef, useState, useCallback } from 'react';
import * as signalR from '@microsoft/signalr';
import { tokenStorage } from '@/services/api';

const API_URL = import.meta.env.VITE_API_URL || 'https://localhost:5001';

interface UseSignalROptions {
  hubPath: string;
  onConnected?: () => void;
  onDisconnected?: () => void;
  onReconnecting?: () => void;
  onReconnected?: () => void;
}

interface SignalRConnection {
  connection: signalR.HubConnection | null;
  connectionState: signalR.HubConnectionState;
  error: Error | null;
  invoke: <T>(methodName: string, ...args: unknown[]) => Promise<T | undefined>;
  on: <T extends unknown[] = unknown[]>(methodName: string, callback: (...args: T) => void) => void;
  off: (methodName: string) => void;
}

export function useSignalR({
  hubPath,
  onConnected,
  onDisconnected,
  onReconnecting,
  onReconnected,
}: UseSignalROptions): SignalRConnection {
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const [connectionState, setConnectionState] = useState<signalR.HubConnectionState>(
    signalR.HubConnectionState.Disconnected
  );
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    const token = tokenStorage.getToken();
    if (!token) {
      setError(new Error('No authentication token available'));
      return;
    }

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_URL}${hubPath}`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(signalR.LogLevel.Information)
      .build();

    connectionRef.current = connection;

    connection.onclose((err) => {
      setConnectionState(signalR.HubConnectionState.Disconnected);
      if (err) {
        setError(err);
      }
      onDisconnected?.();
    });

    connection.onreconnecting((err) => {
      setConnectionState(signalR.HubConnectionState.Reconnecting);
      if (err) {
        setError(err);
      }
      onReconnecting?.();
    });

    connection.onreconnected(() => {
      setConnectionState(signalR.HubConnectionState.Connected);
      setError(null);
      onReconnected?.();
    });

    const startConnection = async () => {
      try {
        setConnectionState(signalR.HubConnectionState.Connecting);
        await connection.start();
        setConnectionState(signalR.HubConnectionState.Connected);
        setError(null);
        onConnected?.();
      } catch (err) {
        setError(err as Error);
        setConnectionState(signalR.HubConnectionState.Disconnected);
        // Retry after 5 seconds
        setTimeout(startConnection, 5000);
      }
    };

    startConnection();

    return () => {
      connection.stop();
    };
  }, [hubPath, onConnected, onDisconnected, onReconnecting, onReconnected]);

  const invoke = useCallback(
    async <T>(methodName: string, ...args: unknown[]): Promise<T | undefined> => {
      if (connectionRef.current?.state !== signalR.HubConnectionState.Connected) {
        console.warn('Cannot invoke method, SignalR not connected');
        return undefined;
      }
      try {
        return await connectionRef.current.invoke<T>(methodName, ...args);
      } catch (err) {
        console.error(`Error invoking ${methodName}:`, err);
        throw err;
      }
    },
    []
  );

  const on = useCallback(<T extends unknown[] = unknown[]>(methodName: string, callback: (...args: T) => void) => {
    connectionRef.current?.on(methodName, callback as (...args: unknown[]) => void);
  }, []);

  const off = useCallback((methodName: string) => {
    connectionRef.current?.off(methodName);
  }, []);

  return {
    connection: connectionRef.current,
    connectionState,
    error,
    invoke,
    on,
    off,
  };
}

export default useSignalR;
