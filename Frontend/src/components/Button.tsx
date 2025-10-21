import React from "react";

/**
 * Reusable button component.
 * - Minimal surface area: children + optional click handler, disabled, className, and type.
 * - Keep styling outside via className; provides a sensible default class.
 */
type ButtonProps = {
    children: React.ReactNode;
    onClick?: () => void;
    disabled?: boolean;
    className?: string;
    type?: "button" | "submit" | "reset";
};

const Button: React.FC<ButtonProps> = ({
    children,
    onClick,
    disabled,
    className,
    type = "button",
}) => {
    // Prefer an explicitly provided className, otherwise fall back to the app's primary button style.
    const buttonClass = className ? className : "primary-button";

    return (
        // Native button element so forms and accessibility behave correctly.
        // onClick is optional; when provided it will be invoked on activation.
        <button
            onClick={onClick}
            disabled={disabled}
            className={buttonClass}
            type={type}
        >
            {children}
        </button>
    );
};

export default Button; // default export for convenient imports