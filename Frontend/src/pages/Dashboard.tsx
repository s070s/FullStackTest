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
                {/* Add client-specific features here */}
            </div>
        );
    } else if (role === 'Trainer') {
        return (
            <div>
                <h2>Trainer Dashboard</h2>
                <p>Welcome, trainer! Manage your clients and workouts here.</p>
                {/* Add trainer-specific features here */}
            </div>
        );
    } else {
        return <div>Unknown role. Please contact support.</div>;
    }
};

export default Dashboard;