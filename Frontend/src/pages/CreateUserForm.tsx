import React, { useState } from "react";
import { createUser } from "../api/api";

export default function CreateUserForm() {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [message, setMessage] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const newUser = await createUser({ username, email });
      setMessage(`✅ User created: ${newUser.username}`);
    } catch (err: any) {
      setMessage(`❌ Error: ${err.message}`);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <input
        placeholder="Username"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
      />
      <input
        placeholder="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
      />
      <button type="submit">Create User</button>
      <p>{message}</p>
    </form>
  );
}
