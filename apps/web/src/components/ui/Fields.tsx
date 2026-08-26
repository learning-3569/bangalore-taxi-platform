import type { InputHTMLAttributes, SelectHTMLAttributes, TextareaHTMLAttributes } from "react";

const fieldClass =
  "mt-1 w-full rounded-md border border-line bg-paper-raised px-3 py-2.5 text-base text-ink shadow-none transition placeholder:text-ink-muted/70 focus:border-brand focus:ring-1 focus:ring-brand";

type FieldProps = {
  id: string;
  label: string;
  hint?: string;
};

export function TextField({
  id,
  label,
  hint,
  ...props
}: FieldProps & InputHTMLAttributes<HTMLInputElement>) {
  return (
    <div>
      <label htmlFor={id} className="text-sm font-medium text-ink">
        {label}
      </label>
      <input id={id} className={fieldClass} {...props} />
      {hint ? <p className="mt-1 text-xs text-ink-muted">{hint}</p> : null}
    </div>
  );
}

export function SelectField({
  id,
  label,
  children,
  ...props
}: FieldProps & SelectHTMLAttributes<HTMLSelectElement>) {
  return (
    <div>
      <label htmlFor={id} className="text-sm font-medium text-ink">
        {label}
      </label>
      <select id={id} className={fieldClass} {...props}>
        {children}
      </select>
    </div>
  );
}

export function TextAreaField({
  id,
  label,
  ...props
}: FieldProps & TextareaHTMLAttributes<HTMLTextAreaElement>) {
  return (
    <div>
      <label htmlFor={id} className="text-sm font-medium text-ink">
        {label}
      </label>
      <textarea id={id} className={fieldClass} {...props} />
    </div>
  );
}
