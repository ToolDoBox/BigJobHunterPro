import type { InputHTMLAttributes, ChangeEvent } from 'react';

interface FormInputProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'onChange'> {
  id: string;
  label: string;
  error?: string;
  onChange: (e: ChangeEvent<HTMLInputElement>) => void;
}

export default function FormInput({
  id,
  label,
  error,
  className = '',
  ...props
}: FormInputProps) {
  return (
    <div className="mb-4">
      <label
        htmlFor={id}
        className="block font-arcade text-xs text-amber mb-2"
      >
        {label}
      </label>

      <input
        id={id}
        className={`
          input-arcade
          ${error ? 'input-arcade-error' : ''}
          ${className}
        `}
        aria-describedby={error ? `${id}-error` : undefined}
        aria-invalid={error ? 'true' : 'false'}
        {...props}
      />

      {error && (
        <p
          id={`${id}-error`}
          className="mt-1 text-sm text-red-500 font-medium"
          role="alert"
        >
          {error}
        </p>
      )}
    </div>
  );
}
