import { forwardRef, type InputHTMLAttributes, type ChangeEvent } from 'react';

interface FormInputProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'onChange'> {
  id: string;
  label: string;
  error?: string;
  onChange: (e: ChangeEvent<HTMLInputElement>) => void;
}

const FormInput = forwardRef<HTMLInputElement, FormInputProps>(({
  id,
  label,
  error,
  className = '',
  ...props
}, ref) => {
  return (
    <div className="mb-4">
      <label
        htmlFor={id}
        className="block font-arcade text-xs text-amber mb-2"
      >
        {label}
      </label>

      <input
        ref={ref}
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
          className="mt-1 text-sm text-red-400 font-medium flex items-start gap-1"
          role="alert"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 20 20"
            fill="currentColor"
            className="w-4 h-4 mt-0.5 flex-shrink-0"
          >
            <path
              fillRule="evenodd"
              d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-8-5a.75.75 0 01.75.75v4.5a.75.75 0 01-1.5 0v-4.5A.75.75 0 0110 5zm0 10a1 1 0 100-2 1 1 0 000 2z"
              clipRule="evenodd"
            />
          </svg>
          <span>{error}</span>
        </p>
      )}
    </div>
  );
});

FormInput.displayName = 'FormInput';

export default FormInput;
