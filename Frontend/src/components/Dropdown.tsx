import React from "react";

type DropdownOption = {
    value: string;
    label: string;
};

interface DropdownProps {
    label?: string;
    name: string;
    value: string;
    options: DropdownOption[];
    onChange: (e: React.ChangeEvent<HTMLSelectElement>) => void;
    required?: boolean;
    style?: React.CSSProperties;
    id?: string;
}

const Dropdown: React.FC<DropdownProps> = ({
    label,
    name,
    value,
    options,
    onChange,
    required = false,
    style,
    id,
}) => (
    <div className="dropdown-container">
        {label && (
            <label className="dropdown-label" htmlFor={id || name}>
                {label}
            </label>
        )}
        <select
            className="dropdown-select"
            id={id || name}
            name={name}
            value={value}
            onChange={onChange}
            required={required}
            style={style}
        >
            {options.map((opt) => (
                <option key={opt.value} value={opt.value}>
                    {opt.label}
                </option>
            ))}
        </select>
    </div>
);

export default Dropdown;