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
}) => (
  <div style={{ marginBottom: 16 }}>
    {label && <label htmlFor={name}>{label}</label>}
    <input
      id={name}
      name={name}
      type={type}
      value={value}
      onChange={onChange}
      required={required}
      placeholder={placeholder}
      style={{ width: "100%", padding: 8, marginTop: 4, ...style }}
    />
  </div>
);

export default InputField;