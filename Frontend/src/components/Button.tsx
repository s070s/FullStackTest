import React from "react";

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
    const buttonClass = className
        ? `primary-button ${className}`
        : "primary-button";
    return (
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

export default Button;