import { useAuthContext } from '@/context/AuthContext';

/**
 * Custom hook for authentication
 * Re-exports AuthContext for cleaner imports
 */
export function useAuth() {
  return useAuthContext();
}

export default useAuth;
