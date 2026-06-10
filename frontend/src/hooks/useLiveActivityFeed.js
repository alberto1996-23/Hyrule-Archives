import { useEffect, useMemo, useState } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { getApiBaseUrl } from '../api/client';

// I use this hook to keep the SignalR connection logic out of page components.
export default function useLiveActivityFeed(initialItems = [], { enabled = true, maxItems = 25 } = {}) {
  const [items, setItems] = useState(initialItems);
  const [connectionStatus, setConnectionStatus] = useState(enabled ? 'connecting' : 'disabled');

  const apiBaseUrl = useMemo(() => getApiBaseUrl(), []);

  useEffect(() => {
    setItems(initialItems);
  }, [initialItems]);

  useEffect(() => {
    if (!enabled) {
      setConnectionStatus('disabled');
      return undefined;
    }

    const connection = new HubConnectionBuilder()
      .withUrl(`${apiBaseUrl}/hubs/activity`, { withCredentials: true })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    connection.onreconnecting(() => setConnectionStatus('reconnecting'));
    connection.onreconnected(() => setConnectionStatus('connected'));
    connection.onclose(() => setConnectionStatus('disconnected'));

    connection.on('ActivityCreated', (activityEvent) => {
      setItems((currentItems) => {
        const deduplicatedItems = currentItems.filter((item) => item.id !== activityEvent.id);
        return [activityEvent, ...deduplicatedItems].slice(0, maxItems);
      });
    });

    async function startConnection() {
      try {
        setConnectionStatus('connecting');
        await connection.start();
        setConnectionStatus('connected');
      } catch {
        setConnectionStatus('error');
      }
    }

    startConnection();

    return () => {
      connection.off('ActivityCreated');
      connection.stop();
    };
  }, [apiBaseUrl, enabled, maxItems]);

  return { items, connectionStatus };
}
