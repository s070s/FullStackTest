import React, { useState, useEffect } from "react";
import InputField from "./InputField";
import Button from "./Button";

type GenericFormDialogProps<T extends object> = {
    open: boolean;
    onClose: () => void;
    onSubmit: (values: T) => void;
    initialValues: Partial<T>;
    fields: Array<{
        name: keyof T;
        label?: string;
        type?: string;
        required?: boolean;
        options?: Array<{ value: any; label: string }>;
    }>;
    title?: string;
};

/**
 * GenericFormDialog
 * - Lightweight, controlled dialog that renders a list of fields defined by props.fields.
 * - Supports text inputs, selects, and checkboxes; easily extended for other types.
 * - Keeps local form state in `values`, initialized from `initialValues` and refreshed
 *   when `initialValues` or `open` change so the dialog can be reused for edit/create flows.
 */
function GenericFormDialog<T extends object>({
    open,
    onClose,
    onSubmit,
    initialValues,
    fields,
    title = "Form",
}: GenericFormDialogProps<T>) {
    // Local form values (partial T). Start from provided initialValues.
    const [values, setValues] = useState<Partial<T>>(initialValues || {});

    // Keep local state in sync when dialog opens or initialValues change.
    useEffect(() => {
        setValues(initialValues || {});
    }, [initialValues, open]);

    // Generic change handler used by all input types.
    const handleChange = (name: keyof T, value: any) => {
        setValues((prev) => ({ ...prev, [name]: value }));
    };

    // Prevent default form submit and forward typed values to caller.
    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSubmit(values as T);
    };

    if (!open) return null;

    return (
        <div className="dialog-backdrop">
            <div className="dialog">
                <h2>{title}</h2>
                <form onSubmit={handleSubmit}>
                    {fields.map((field) => (
                        <div key={String(field.name)}>
                            {field.options ? (
                                // Render a select when options are provided.
                                <label>
                                    {field.label || String(field.name)}
                                    {field.required && <span style={{ color: "red" }}> *</span>}
                                    <select
                                        className="dialog-select"
                                        // Prefer existing value, fall back to first option or empty string.
                                        value={(values[field.name] as any) || field.options?.[0]?.value || ""}
                                        onChange={e => handleChange(field.name, e.target.value)}
                                        required={field.required}
                                    >
                                        {field.options.map(opt => (
                                            <option key={opt.value} value={opt.value}>{opt.label}</option>
                                        ))}
                                    </select>
                                </label>
                            ) : field.type === "checkbox" ? (
                                // Checkbox input expects boolean values.
                                <label>
                                    {field.label || String(field.name)}
                                    {field.required && <span style={{ color: "red" }}> *</span>}
                                    <input
                                        type="checkbox"
                                        checked={!!values[field.name]}
                                        onChange={e => handleChange(field.name, e.target.checked)}
                                        name={String(field.name)}
                                    />
                                </label>
                            ) : (
                                // Default to our InputField for text/date/password/etc.
                                <InputField
                                    label={field.label || String(field.name)}
                                    type={field.type || "text"}
                                    name={String(field.name)}
                                    value={(values[field.name] as any) || ""}
                                    onChange={e => handleChange(field.name, e.target.value)}
                                    required={field.required}
                                    showPasswordToggle={field.name === "password"}
                                />
                            )}
                        </div>
                    ))}
                    <div style={{ display: "flex", justifyContent: "flex-end", gap: 8 }}>
                        <Button type="button" onClick={onClose}>
                            Cancel
                        </Button>
                        <Button type="submit">
                            Submit
                        </Button>
                    </div>
                </form>
            </div>
        </div>
    );
}

export default GenericFormDialog;
