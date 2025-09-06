import React, { useState } from "react";
import { registerUser } from "../api/api";
import type { RegisterUserDto } from "../api/api";
import Button from "../components/Button";
import InputField from "../components/InputField";
import ErrorMessage from "../components/ErrorMessage";
import "../css/ContainerRules.css"; // Import CSS

const RegisterPage: React.FC = () => {
  // States
  const [form, setForm] = useState<RegisterUserDto>({
    username: "",
    email: "",
    password: "",
    role: "Client",
  });
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<boolean>(false);

  // Handle input changes for form fields
  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };
  // Handle form submission
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    try {
      await registerUser(form);
      setSuccess(true);
    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <div className="form-container">
      <h2>Account Registration</h2>
      {success ? (
        <div>Success! You can now log in.</div>
      ) : (
        <form onSubmit={handleSubmit}>
          <InputField
            label="Username"
            name="username"
            value={form.username}
            onChange={handleChange}
            required
            placeholder="Enter your username"
          />
          <InputField
            label="Email"
            name="email"
            type="email"
            value={form.email}
            onChange={handleChange}
            required
            placeholder="Enter your email"
          />
          <InputField
            label="Password"
            name="password"
            type="password"
            value={form.password}
            onChange={handleChange}
            required
            placeholder="Enter your password"
            showPasswordToggle={true}
          />
          <div style={{ marginBottom: 16 }}>
            <label htmlFor="role">Role</label>
            <select
              id="role"
              name="role"
              value={form.role}
              onChange={handleChange}
              style={{ width: "100%", padding: 8, marginTop: 4 }}
            >
              <option value="Client">Client</option>
              <option value="Trainer">Trainer</option>
            </select>
          </div>
          <Button type="submit" className="primary-button full-width">
            Register
          </Button>
        </form>
      )}
      {error && <ErrorMessage message={error} />}
    </div>
  );
};

export default RegisterPage;