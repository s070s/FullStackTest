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

function GenericFormDialog<T extends object>({
    open,
    onClose,
    onSubmit,
    initialValues,
    fields,
    title = "Form",
}: GenericFormDialogProps<T>) {
    const [values, setValues] = useState<Partial<T>>(initialValues || {});

    // Add this effect to update values when initialValues change
    useEffect(() => {
        setValues(initialValues || {});
    }, [initialValues, open]);

    const handleChange = (name: keyof T, value: any) => {
        setValues((prev) => ({ ...prev, [name]: value }));
    };

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
                                <label>
                                    {field.label || String(field.name)}
                                    {field.required && <span style={{ color: "red" }}> *</span>}
                                    <select
                                        className="dialog-select"
                                        value={values[field.name] as any ||field.options?.[0]?.value || ""}
                                        onChange={e => handleChange(field.name, e.target.value)}
                                        required={field.required}
                                    >
                                        {field.options.map(opt => (
                                            <option key={opt.value} value={opt.value}>{opt.label}</option>
                                        ))}
                                    </select>
                                </label>
                            ) : field.type === "checkbox" ? (
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
                                <InputField
                                    label={field.label || String(field.name)}
                                    type={field.type || "text"}
                                    name={String(field.name)}
                                    value={values[field.name] as any || ""}
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
