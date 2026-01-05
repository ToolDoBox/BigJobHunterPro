import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '@/hooks/useAuth';
import FormInput from '@/components/forms/FormInput';
import FormError from '@/components/forms/FormError';

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

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    // Display name validation
    if (!displayName.trim()) {
      newErrors.displayName = 'Display name is required';
    } else if (displayName.trim().length < 2) {
      newErrors.displayName = 'Display name must be at least 2 characters';
    }

    // Email validation
    if (!email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      newErrors.email = 'Please enter a valid email address';
    }

    // Password validation
    if (!password) {
      newErrors.password = 'Password is required';
    } else if (password.length < 8) {
      newErrors.password = 'Password must be at least 8 characters';
    } else if (!/\d/.test(password)) {
      newErrors.password = 'Password must contain at least one digit';
    } else if (!/[A-Z]/.test(password)) {
      newErrors.password = 'Password must contain at least one uppercase letter';
    }

    // Confirm password validation
    if (!confirmPassword) {
      newErrors.confirmPassword = 'Please confirm your password';
    } else if (password !== confirmPassword) {
      newErrors.confirmPassword = 'Passwords do not match';
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

          {/* General error message */}
          {errors.general && (
            <FormError message={errors.general} className="mb-4" />
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

          {/* Password field */}
          <FormInput
            id="password"
            label="Password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            error={errors.password}
            placeholder="Min 8 chars, 1 digit, 1 uppercase"
            autoComplete="new-password"
          />

          {/* Confirm password field */}
          <FormInput
            id="confirmPassword"
            label="Confirm Password"
            type="password"
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

        {/* Password requirements hint */}
        <div className="mt-4 text-center text-xs text-gray-500">
          <p>Password must contain:</p>
          <p>At least 8 characters, 1 digit, 1 uppercase letter</p>
        </div>
      </div>
    </div>
  );
}
