import React from "react";
/**
 * Simple CSS-only loading spinner.
 * - Styles/animation live in CSS.
 * - Keep styling/size in CSS (class names provided).
 */
const LoadingSpinner: React.FC = () => {
  return (
    <div className="loading-spinner-container ">
      <div className="loading-spinner"></div>
    </div>
  );
};

export default LoadingSpinner;
