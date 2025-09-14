import React, { useEffect, useState } from "react";
import TableGeneric from "../../components/TableGeneric";
import { adminFetchAllUsers } from "../../utils/api/api";
import type { UserDto } from "../../utils/data/userdtos";
import { useAuth } from "../../utils/contexts/AuthContext";


const AdminDashboard: React.FC = () => {

    const [users, setUsers] = useState<UserDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const { token } = useAuth();

    useEffect(() => {
        adminFetchAllUsers(token)
            .then(setUsers)
            .catch((err) => setError(err.message))
            .finally(() => setLoading(false));
    }, [token]);
    return (
        <div>
            <h2>Admin Dashboard</h2>
            <p>Welcome, admin! Manage users, trainers, and system settings here.</p>
            <section>
                <h3>User Management</h3>
                {loading ? (
                    <div>Loading...</div>
                ) : error ? (
                    <div>Error: {error}</div>
                ) : (
                    <TableGeneric data={users} />
                )}
            </section>
            <section>
                <h3>System Statistics</h3>
                <div>
                    <span>Total Users: {users.length}</span><br />
                </div>
            </section>
            <section>
                <h3>Settings</h3>
                <div>
                    <span>System Status: [Online]</span>
                </div>
            </section>
            <section>
                <h3>Messages</h3>
                <div>
                    <span>No new messages.</span>
                </div>
            </section>
        </div>
    );
};

export default AdminDashboard;