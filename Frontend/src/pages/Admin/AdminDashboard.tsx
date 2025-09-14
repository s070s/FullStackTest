import React from 'react';

const AdminDashboard: React.FC = () => {
    return (
        <div>
            <h2>Admin Dashboard</h2>
            <p>Welcome, admin! Manage users, trainers, and system settings here.</p>
            <section>
                <h3>User Management</h3>
                <ul>
                    <li>[User Name] - [Role]</li>
                    <li>[User Name] - [Role]</li>
                </ul>
            </section>
            <section>
                <h3>System Statistics</h3>
                <div>
                    <span>Total Users: [0]</span><br />
                    <span>Active Trainers: [0]</span>
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