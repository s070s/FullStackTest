import React from 'react';

// Helper to decode JWT and get payload
function parseJwt(token: string) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split('')
                .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        );
        return JSON.parse(jsonPayload);
    } catch {
        return null;
    }
}

const Dashboard: React.FC = () => {
    const token = localStorage.getItem('authToken');
    if (!token) {
        return <div>Please log in to view your dashboard.</div>;
    }

    const payload = parseJwt(token);
    const role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]; // Adjust this if your JWT uses a different claim name

    if (role === 'Client') {
        return (
            <div>
                <h2>Client Dashboard</h2>
                <p>Welcome to your fitness dashboard!</p>
                {/* Client Skeleton */}
                <section>
                    <h3>Profile</h3>
                    <div>
                        <span>Name: [Your Name]</span><br />
                        <span>Email: [Your Email]</span>
                    </div>
                </section>
                <section>
                    <h3>Progress</h3>
                    <div>
                        <span>Workouts Completed: [0]</span><br />
                        <span>Calories Burned: [0]</span>
                    </div>
                </section>
                <section>
                    <h3>Upcoming Workouts</h3>
                    <ul>
                        <li>[Workout Name] - [Date]</li>
                        <li>[Workout Name] - [Date]</li>
                    </ul>
                </section>
                <section>
                    <h3>Messages</h3>
                    <div>
                        <span>No new messages.</span>
                    </div>
                </section>
            </div>
        );
    } else if (role === 'Trainer') {
        return (
            <div>
                <h2>Trainer Dashboard</h2>
                <p>Welcome, trainer! Manage your clients and workouts here.</p>
                {/* Trainer Skeleton */}
                <section>
                    <h3>Your Clients</h3>
                    <ul>
                        <li>[Client Name] - [Status]</li>
                        <li>[Client Name] - [Status]</li>
                    </ul>
                </section>
                <section>
                    <h3>Workouts</h3>
                    <ul>
                        <li>[Workout Name] - [Date]</li>
                        <li>[Workout Name] - [Date]</li>
                    </ul>
                </section>
                <section>
                    <h3>Messages</h3>
                    <div>
                        <span>No new messages.</span>
                    </div>
                </section>
            </div>
        );
    } else {
        return <div>Unknown role. Please contact support.</div>;
    }
};

export default Dashboard;