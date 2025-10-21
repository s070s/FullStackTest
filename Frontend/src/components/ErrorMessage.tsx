import React from "react";

/**
 * Small inline error message component.
 * - Renders a single message string when provided, otherwise renders nothing.
 * - Styling is intentionally minimal and inline so it displays consistently without extra CSS;
 *   callers can replace/override styling if needed.
 */
const ErrorMessage: React.FC<{ message: string }> = ({ message }) =>
  message ? <div style={{ color: "red", marginBottom: 16 }}>{message}</div> : null;

export default ErrorMessage;