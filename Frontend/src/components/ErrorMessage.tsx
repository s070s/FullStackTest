import React from "react";

const ErrorMessage: React.FC<{ message: string }> = ({ message }) =>
  message ? <div style={{ color: "red", marginBottom: 16 }}>{message}</div> : null;

export default ErrorMessage;