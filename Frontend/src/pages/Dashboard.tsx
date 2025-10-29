import React from 'react';
import { parseJwt } from '../utils/jwtauth/jwtauth';
import AdminDashboard from './Admin/AdminDashboard';
import ClientDashboard from './Client/ClientDashboard';
import TrainerDashboard from './Trainer/TrainerDashboard';
import { useAuth } from '../utils/contexts/AuthContext';

const Dashboard: React.FC = () => {
    const { token } = useAuth();
    if (!token) {
        return <div>Please log in to view your dashboard.</div>;
    }

    const payload = parseJwt(token);
    const role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]; // Adjust this if your JWT uses a different claim name
    /** Conditional Logic for Dashboards */
    if (role === 'Client') {
        return <ClientDashboard />;
    } else if (role === 'Trainer') {
        return <TrainerDashboard />;
    } else if (role === 'Admin') {
        return <AdminDashboard />;
    } else {
        return <div>Unknown role. Please contact support.</div>;
    }
};

export default Dashboard;