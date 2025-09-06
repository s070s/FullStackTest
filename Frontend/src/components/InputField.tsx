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
    <div style={{ marginBottom: 16, position: "relative" }}>
      {label && <label htmlFor={name}>{label}</label>}
      <input
        id={name}
        name={name}
        type={isPassword && showPassword ? "text" : type}
        value={value}
        onChange={onChange}
        required={required}
        placeholder={placeholder}
        style={{ width: "100%", padding: 8, marginTop: 4, ...style }}
      />
      {isPassword && showPasswordToggle && (
        <button
          type="button"
          onClick={() => setShowPassword((prev) => !prev)}
          style={{
            position: "absolute",
            right: 0,
            top: 30,
            width: 32,
            height: 32,
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            background: "transparent",
            border: "none",
            borderRadius: 6,
            cursor: "pointer",
            padding: 0,
            fontSize: 18,
            transition: "background 0.2s",
            boxShadow: "0 1px 4px rgba(0,0,0,0.04)",
          }}
          onMouseOver={e => e.currentTarget.style.background = "#f0f0f0"}
          onMouseOut={e => e.currentTarget.style.background = "transparent"}
          aria-label={showPassword ? "Hide password" : "Show password"}
        >
          <i className={showPassword ? "fa fa-eye-slash" : "fa fa-eye"}></i>
        </button>
      )}
    </div>
  );
};

export default InputField;