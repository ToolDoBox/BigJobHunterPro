import { useState, type FormEvent } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '@/hooks/useAuth';
import FormInput from '@/components/forms/FormInput';
import FormError from '@/components/forms/FormError';

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

          {/* General error message */}
          {errors.general && (
            <FormError message={errors.general} className="mb-4" />
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

          {/* Password field */}
          <FormInput
            id="password"
            label="Password"
            type="password"
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
      </div>
    </div>
  );
}
