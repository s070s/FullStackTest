import React, { useState } from "react";
import { registerUser } from "../api/api";
import type { RegisterUserDto } from "../api/api";

export default function RegisterPage() {
  const [form, setForm] = useState<RegisterUserDto>({
    username: "",
    email: "",
    password: "",
    role: "Client",
  });
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<boolean>(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

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
    <div>
      <h2>Account Registration</h2>
      {success ? (
        <div>Success! You can now log in.</div>
      ) : (
        <form onSubmit={handleSubmit}>
          <input
            name="username"
            placeholder="Username"
            value={form.username}
            onChange={handleChange}
            required
          />
          <input
            name="email"
            type="email"
            placeholder="Email"
            value={form.email}
            onChange={handleChange}
            required
          />
          <input
            name="password"
            type="password"
            placeholder="Password"
            value={form.password}
            onChange={handleChange}
            required
          />
          <select name="role" value={form.role} onChange={handleChange}>
            <option value="Client">Client</option>
            <option value="Trainer">Trainer</option>
          </select>
          <button type="submit">Register</button>
        </form>
      )}
      {error && <div style={{ color: "red" }}>{error}</div>}
    </div>
  );
}