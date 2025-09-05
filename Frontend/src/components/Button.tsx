import React from "react";

type ButtonProps = {
    children: React.ReactNode;
    onClick?: () => void;
    disabled?: boolean;
    className?: string;
};

const Button: React.FC<ButtonProps> = ({ children, onClick, disabled, className }) => {
    return (
        <button
            onClick={onClick}
            disabled={disabled}
            className={className}
        >
            {children}
        </button>
    );
};

export default Button;