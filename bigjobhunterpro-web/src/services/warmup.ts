import api from './api';

interface WarmupOptions {
  maxRetries?: number;
  initialTimeout?: number;
  maxTimeout?: number;
}

const DEFAULT_OPTIONS: Required<WarmupOptions> = {
  maxRetries: 3,
  initialTimeout: 10000, // 10 seconds
  maxTimeout: 30000, // 30 seconds
};

let warmupInProgress = false;
let lastWarmupAttempt = 0;
const WARMUP_COOLDOWN_MS = 60000; // Don't retry warmup more than once per minute

/**
 * Attempts to warm up the API server with exponential backoff.
 * Designed for Azure free tier cold starts (up to 30 seconds).
 *
 * @param options - Configuration for warmup behavior
 * @returns Promise that resolves when server responds or max retries reached
 */
export async function warmupServer(options: WarmupOptions = {}): Promise<boolean> {
  const opts = { ...DEFAULT_OPTIONS, ...options };

  // Prevent concurrent warmup attempts
  if (warmupInProgress) {
    console.log('[Warmup] Already in progress, skipping...');
    return false;
  }

  // Cooldown: prevent too frequent warmup attempts
  const now = Date.now();
  if (now - lastWarmupAttempt < WARMUP_COOLDOWN_MS) {
    console.log('[Warmup] Cooldown active, skipping...');
    return false;
  }

  warmupInProgress = true;
  lastWarmupAttempt = now;

  console.log('[Warmup] Starting server warmup...');

  for (let attempt = 1; attempt <= opts.maxRetries; attempt++) {
    try {
      // Calculate timeout for this attempt (exponential backoff)
      const timeout = Math.min(
        opts.initialTimeout * Math.pow(2, attempt - 1),
        opts.maxTimeout
      );

      console.log(`[Warmup] Attempt ${attempt}/${opts.maxRetries} (timeout: ${timeout}ms)`);

      // Ping the health endpoint
      await api.get('/api/health', {
        timeout,
        // Don't send auth headers for warmup to avoid unnecessary processing
        headers: { Authorization: undefined }
      });

      console.log('[Warmup] Server is responsive!');
      warmupInProgress = false;
      return true;

    } catch (error: any) {
      const isLastAttempt = attempt === opts.maxRetries;

      if (isLastAttempt) {
        console.warn('[Warmup] Max retries reached. Server may still be cold starting.');
      } else {
        console.log(`[Warmup] Attempt ${attempt} failed, will retry...`);
      }
    }
  }

  warmupInProgress = false;
  return false;
}

/**
 * Initiates warmup only if conditions are met (not too recent, not already in progress).
 * Safe to call multiple times.
 */
export function initiateWarmup(): void {
  warmupServer().catch(err => {
    console.error('[Warmup] Error during warmup:', err);
  });
}

/**
 * Check if warmup is currently running
 */
export function isWarmupInProgress(): boolean {
  return warmupInProgress;
}
