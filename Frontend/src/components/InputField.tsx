import React from "react";

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
  const [showPassword, setShowPassword] = React.useState(false);
  const isPassword = type === "password";
  return (
    <div className="input-field-container" style={style}>
      {label && <label htmlFor={name} className="input-field-label">{label}</label>}
      <input
        id={name}
        name={name}
        type={isPassword && showPassword ? "text" : type}
        value={type === "date" && value ? value.split("T")[0] : value}
        onChange={onChange}
        required={required}
        placeholder={placeholder}
        className="input-field-input"
      />
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