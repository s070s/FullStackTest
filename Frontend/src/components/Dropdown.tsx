import React from "react";

/**
 * Simple, accessible select control.
 * - Keeps markup minimal so styling can be applied externally via className.
 * - Exposes value and onChange to make it a controlled component.
 */
type DropdownOption = {
    value: string;
    label: string;
};

interface DropdownProps {
    label?: string; // Optional visible label (linked to the select for accessibility)
    name: string; // input name
    value: string; // current value (controlled)
    options: DropdownOption[]; // available options
    onChange: (e: React.ChangeEvent<HTMLSelectElement>) => void; // change handler
    required?: boolean;
    style?: React.CSSProperties;
    id?: string; // optional id to link label/select; falls back to name
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
            // htmlFor links label to the select for screen readers
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
            aria-label={label ? undefined : name} // provide fallback accessible name
        >
            {options.map((opt) => (
                // option.value used as key and value; keep label for display
                <option key={opt.value} value={opt.value}>
                    {opt.label}
                </option>
            ))}
        </select>
    </div>
);

export default Dropdown;