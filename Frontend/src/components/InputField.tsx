import React from "react";

/**
 * Small, focused input component used across the app.
 * - Renders a label (when provided) and a single input element.
 * - Supports common types (text, password, date, etc.). Date values are normalized to YYYY-MM-DD.
 * - Optional password visibility toggle when showPasswordToggle is true.
 * - Styling is kept to classNames so visual rules live in CSS.
 */
interface InputFieldProps {
  label?: string;
  name: string;
  type?: string;
  value: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  required?: boolean;
  placeholder?: string;
  style?: React.CSSProperties;
  showPasswordToggle?: boolean;
}

const InputField: React.FC<InputFieldProps> = ({
  label,
  name,
  type = "text",
  value,
  onChange,
  required,
  placeholder,
  style,
  showPasswordToggle,
}) => {
  // Local toggle state only used for password fields to switch visibility.
  const [showPassword, setShowPassword] = React.useState(false);
  const isPassword = type === "password";

  return (
    <div className="input-field-container" style={style}>
      {/*
        Link label -> input with htmlFor/id for accessibility.
        Label is optional; when omitted the input should be given an accessible name by its context.
      */}
      {label && <label htmlFor={name} className="input-field-label">{label}</label>}

      <input
        id={name}
        name={name}
        // When password + toggled showPassword, reveal as text; otherwise use provided type.
        type={isPassword && showPassword ? "text" : type}
        // Normalize date inputs to YYYY-MM-DD if a full ISO string is passed.
        value={type === "date" && value ? value.split("T")[0] : value}
        onChange={onChange}
        required={required}
        placeholder={placeholder}
        className="input-field-input"
      />

      {/*
        Optional button to toggle password visibility.
        Uses aria-label so screen readers get the current action.
      */}
      {isPassword && showPasswordToggle && (
        <button
          type="button"
          onClick={() => setShowPassword((prev) => !prev)}
          className="input-field-password-toggle"
          aria-label={showPassword ? "Hide password" : "Show password"}
        >
          <i className={showPassword ? "fa fa-eye-slash" : "fa fa-eye"}></i>
        </button>
      )}
    </div>
  );
};

export default InputField;