/** Registration Page */

import React, { useState } from "react";
import { registerUser } from "../utils/api/api";
import type { RegisterUserDto } from "../utils/data/userdtos";
import Button from "../components/Button";
import InputField from "../components/InputField";
import ErrorMessage from "../components/ErrorMessage";
import Dropdown from "../components/Dropdown";
import LoadingSpinner from "../components/LoadingSpinner"; // <-- import spinner


const RegisterPage: React.FC = () => {
  // States
  const [form, setForm] = useState<RegisterUserDto>({
    username: "",
    email: "",
    password: "",
    role: "Client",
  });
  const roleOptions = [
    { value: "Client", label: "Client" },
    { value: "Trainer", label: "Trainer" },
  ];
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<boolean>(false);
  const [loading, setLoading] = useState(false);

  // Handle input changes for form fields
  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };
  // Handle form submission
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      await registerUser(form);
      setSuccess(true);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="form-container">
      <h2>Account Registration</h2>
      {success ? (
        <div>Success! You can now log in.</div>
      ) : loading ? (
        /** Loading Spinner while the form is submitted*/
        <LoadingSpinner />
      ) : (
        /** Registration Form */
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
          {/** Role Selection */}
          <Dropdown
            label="Role"
            name="role"
            value={form.role ?? ""}
            options={roleOptions}
            onChange={handleChange}
          />
          {/** Submit Button */}
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