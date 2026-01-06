import { useState, type FormEvent } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '@/hooks/useAuth';
import FormInput from '@/components/forms/FormInput';
import FormError from '@/components/forms/FormError';
import PasswordInput from '@/components/forms/PasswordInput';

interface FormErrors {
  email?: string;
  password?: string;
  general?: string;
}

export default function Login() {
  const navigate = useNavigate();
  const location = useLocation();
  const { login, isLoading, isAuthenticated } = useAuth();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [errors, setErrors] = useState<FormErrors>({});

  // Redirect if already authenticated
  if (isAuthenticated) {
    const from = (location.state as { from?: { pathname: string } })?.from?.pathname || '/app';
    navigate(from, { replace: true });
    return null;
  }

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    // Email validation with detailed feedback
    if (!email.trim()) {
      newErrors.email = 'Please enter your email address to log in';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      if (!email.includes('@')) {
        newErrors.email = 'Email must contain an @ symbol (e.g., hunter@example.com)';
      } else if (!email.split('@')[1]?.includes('.')) {
        newErrors.email = 'Email must contain a domain with a period (e.g., @example.com)';
      } else {
        newErrors.email = 'Please enter a valid email address (e.g., hunter@example.com)';
      }
    }

    // Password validation
    if (!password) {
      newErrors.password = 'Please enter your password to log in';
    } else if (password.length < 8) {
      newErrors.password = 'Password must be at least 8 characters';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    // Clear previous errors
    setErrors({});

    // Validate form
    if (!validateForm()) {
      return;
    }

    try {
      await login(email, password);

      // Redirect to intended destination or dashboard
      const from = (location.state as { from?: { pathname: string } })?.from?.pathname || '/app';
      navigate(from, { replace: true });
    } catch (error) {
      // Handle API errors
      const message = error instanceof Error
        ? error.message
        : 'Login failed. Please check your credentials.';

      setErrors({ general: message });
    }
  };

  return (
    <div className="min-h-screen bg-metal-dark app-shell flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        {/* Logo/Title */}
        <div className="text-center mb-8">
          <h1 className="font-arcade text-xl md:text-2xl text-blaze mb-2">
            BIG JOB HUNTER PRO
          </h1>
          <p className="font-arcade text-xs text-terminal">
            // HUNTER LOGIN //
          </p>
        </div>

        {/* Login form - Metal panel with screws */}
        <form
          onSubmit={handleSubmit}
          className="metal-panel metal-panel-orange"
        >
          {/* Bottom screws */}
          <div className="metal-panel-screws" />

          <h2 className="font-arcade text-base text-amber mb-6 text-center">
            ACCESS THE LODGE
          </h2>

          {/* General error message with enhanced styling for multi-line errors */}
          {errors.general && (
            <div className="mb-4 p-3 bg-red-900/20 border border-red-500/50 rounded">
              <p className="text-sm text-red-400 whitespace-pre-line flex items-start gap-2">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                  className="w-5 h-5 mt-0.5 flex-shrink-0"
                >
                  <path
                    fillRule="evenodd"
                    d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-8-5a.75.75 0 01.75.75v4.5a.75.75 0 01-1.5 0v-4.5A.75.75 0 0110 5zm0 10a1 1 0 100-2 1 1 0 000 2z"
                    clipRule="evenodd"
                  />
                </svg>
                <span>{errors.general}</span>
              </p>
            </div>
          )}

          {/* Email field */}
          <FormInput
            id="email"
            label="Email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            error={errors.email}
            placeholder="hunter@example.com"
            autoComplete="email"
            autoFocus
          />

          {/* Password field with visibility toggle */}
          <PasswordInput
            id="password"
            label="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            error={errors.password}
            placeholder="Enter your password"
            autoComplete="current-password"
          />

          {/* Submit button */}
          <button
            type="submit"
            disabled={isLoading}
            className="w-full btn-metal-primary mt-6"
          >
            {isLoading ? 'AUTHENTICATING...' : 'LOCK & LOAD'}
          </button>

          {/* Register link */}
          <p className="text-center mt-6 text-gray-400">
            New hunter?{' '}
            <Link
              to="/register"
              className="text-amber hover:text-blaze transition-colors font-semibold"
            >
              Create Account
            </Link>
          </p>
        </form>

        {/* Helpful tips section */}
        <div className="mt-4 text-center text-xs text-gray-500">
          <p className="font-semibold mb-1">💡 Login Tips:</p>
          <p>• Use the 👁️ icon to verify your password</p>
          <p>• Email addresses are case-insensitive</p>
          <p>• Account locked? Wait 5 minutes after multiple failed attempts</p>
        </div>

        {/* Footer */}
        <div className="mt-8 text-center space-y-1">
          <div className="text-xs text-terminal">
            Big Job Hunter Pro v0.1
          </div>
          <div className="text-xs text-gray-400">
            Crafted in co-op mode by{' '}
            <a
              href="https://christianadleta.com"
              target="_blank"
              rel="noopener noreferrer"
              className="text-amber hover:text-blaze transition-colors"
            >
              Christian Adleta
            </a>
            {' & '}
            <a
              href="https://emmettshaughnessy.com"
              target="_blank"
              rel="noopener noreferrer"
              className="text-amber hover:text-blaze transition-colors"
            >
              Emmett Shaughnessy
            </a>
          </div>
        </div>
      </div>
    </div>
  );
}
