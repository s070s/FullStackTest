import React from "react";

const FormContainer: React.FC<{ children: React.ReactNode }> = ({ children }) => (
  <div style={{ maxWidth: 400, margin: "40px auto", padding: 24, border: "1px solid #ddd", borderRadius: 8 }}>
    {children}
  </div>
);

export default FormContainer;