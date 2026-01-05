interface FormErrorProps {
  message: string;
  className?: string;
}

export default function FormError({ message, className = '' }: FormErrorProps) {
  return (
    <div
      className={`
        bg-red-900/30 border-2 border-red-500 p-3
        text-red-400 text-sm font-medium
        ${className}
      `}
      role="alert"
    >
      <span className="font-arcade text-xs text-red-500 mr-2">ERROR:</span>
      {message}
    </div>
  );
}
