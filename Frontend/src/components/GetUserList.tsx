import { useEffect, useState } from "react";
import { getAllUsers } from "../api/api";
import type { UserDto } from "../api/api";

export default function UserList() {


  const [users, setUsers] = useState<UserDto[]>([]);
  const [error, setError] = useState("");



  async function loadUsers() {
    try {
      const data = await getAllUsers();
      setUsers(data);
    }
    catch (err: any) {
      setError("Failed to load users");
    }
  }

  useEffect(() => {
    loadUsers();
  }, []);

if (error) return <p>❌ Error: {error}</p>;

  return (
    <div>
      <h2>Users</h2>
      <button onClick={loadUsers} style={{ marginBottom: '1em' }}>Refresh</button>
      <ul>
        {users.map((u) => (
          <li key={u.id}>
            {u.username} — {u.email} — {u.isActive ? "Active" : "Inactive"}
          </li>
        ))}
      </ul>
    </div>
  );
}