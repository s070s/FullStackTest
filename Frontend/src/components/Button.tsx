import React from "react";


type ButtonProps = {
    children: React.ReactNode;
    onClick?: () => void;
    disabled?: boolean;
    className?: string;
};

function Button({ children, onClick, disabled, className }: ButtonProps) {
    return (
        <button
            onClick={onClick}
            disabled={disabled}
            className={className}
        >
            {children}
        </button>
    );
}

export default Button;