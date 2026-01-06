import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '@/hooks/useAuth';
import FormInput from '@/components/forms/FormInput';
import FormError from '@/components/forms/FormError';
import PasswordInput, { type PasswordRequirement } from '@/components/forms/PasswordInput';

interface FormErrors {
  displayName?: string;
  email?: string;
  password?: string;
  confirmPassword?: string;
  general?: string;
}

export default function Register() {
  const navigate = useNavigate();
  const { register, isLoading, isAuthenticated } = useAuth();

  const [displayName, setDisplayName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [errors, setErrors] = useState<FormErrors>({});

  // Redirect if already authenticated
  if (isAuthenticated) {
    navigate('/app', { replace: true });
    return null;
  }

  // Password requirements for real-time validation
  const passwordRequirements: PasswordRequirement[] = [
    { label: 'At least 8 characters', test: (pwd) => pwd.length >= 8 },
    { label: 'Contains at least one digit (0-9)', test: (pwd) => /\d/.test(pwd) },
    { label: 'Contains at least one uppercase letter (A-Z)', test: (pwd) => /[A-Z]/.test(pwd) },
    { label: 'Contains at least one lowercase letter (a-z)', test: (pwd) => /[a-z]/.test(pwd) },
  ];

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    // Display name validation with detailed feedback
    if (!displayName.trim()) {
      newErrors.displayName = 'Display name is required to create your hunter profile';
    } else if (displayName.trim().length < 2) {
      newErrors.displayName = 'Display name must be at least 2 characters (current: ' + displayName.trim().length + ')';
    } else if (displayName.trim().length > 50) {
      newErrors.displayName = 'Display name must be less than 50 characters (current: ' + displayName.trim().length + ')';
    }

    // Email validation with detailed feedback
    if (!email.trim()) {
      newErrors.email = 'Email address is required to create your account';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      if (!email.includes('@')) {
        newErrors.email = 'Email must contain an @ symbol (e.g., hunter@example.com)';
      } else if (!email.split('@')[1]?.includes('.')) {
        newErrors.email = 'Email must contain a domain with a period (e.g., @example.com)';
      } else {
        newErrors.email = 'Please enter a valid email address (e.g., hunter@example.com)';
      }
    }

    // Password validation - check all requirements
    if (!password) {
      newErrors.password = 'Password is required to secure your account';
    } else {
      const failedRequirements = passwordRequirements
        .filter(req => !req.test(password))
        .map(req => req.label);

      if (failedRequirements.length > 0) {
        newErrors.password = 'Password missing: ' + failedRequirements.join(', ');
      }
    }

    // Confirm password validation with detailed feedback
    if (!confirmPassword) {
      newErrors.confirmPassword = 'Please re-enter your password to confirm';
    } else if (password !== confirmPassword) {
      newErrors.confirmPassword = 'Passwords do not match - please make sure both passwords are identical';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    setErrors({});

    if (!validateForm()) {
      return;
    }

    try {
      // Register and auto-login
      await register(email, password, displayName);

      // Redirect to dashboard
      navigate('/app', { replace: true });
    } catch (error) {
      const message = error instanceof Error
        ? error.message
        : 'Registration failed. Please try again.';

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
            // NEW HUNTER REGISTRATION //
          </p>
        </div>

        {/* Register form - Metal panel with screws */}
        <form
          onSubmit={handleSubmit}
          className="metal-panel metal-panel-orange"
        >
          {/* Bottom screws */}
          <div className="metal-panel-screws" />

          <h2 className="font-arcade text-base text-amber mb-6 text-center">
            JOIN THE HUNT
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

          {/* Display name field */}
          <FormInput
            id="displayName"
            label="Hunter Name"
            type="text"
            value={displayName}
            onChange={(e) => setDisplayName(e.target.value)}
            error={errors.displayName}
            placeholder="Your display name"
            autoComplete="name"
            autoFocus
          />

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
          />

          {/* Password field with requirements */}
          <PasswordInput
            id="password"
            label="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            error={errors.password}
            placeholder="Create a strong password"
            autoComplete="new-password"
            showRequirements={true}
            requirements={passwordRequirements}
          />

          {/* Confirm password field */}
          <PasswordInput
            id="confirmPassword"
            label="Confirm Password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            error={errors.confirmPassword}
            placeholder="Re-enter your password"
            autoComplete="new-password"
          />

          {/* Submit button */}
          <button
            type="submit"
            disabled={isLoading}
            className="w-full btn-metal-primary mt-6"
          >
            {isLoading ? 'CREATING ACCOUNT...' : 'CREATE HUNTER PROFILE'}
          </button>

          {/* Login link */}
          <p className="text-center mt-6 text-gray-400">
            Already a hunter?{' '}
            <Link
              to="/login"
              className="text-amber hover:text-blaze transition-colors font-semibold"
            >
              Sign In
            </Link>
          </p>
        </form>

        {/* Password requirements hint (static) */}
        <div className="mt-4 text-center text-xs text-gray-500">
          <p className="font-semibold mb-1">🔒 Password Security Requirements:</p>
          <p>At least 8 characters • 1 digit • 1 uppercase • 1 lowercase</p>
          <p className="mt-1 text-gray-600">Tip: Use the 👁️ icon to view your password</p>
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
