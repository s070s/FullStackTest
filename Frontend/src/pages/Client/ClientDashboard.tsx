import React from 'react';
import { useAuth } from "../../utils/contexts/AuthContext";

const ClientDashboard: React.FC = () => {
    const { currentUser } = useAuth();
    return (
        <div>
            <h2>Client Dashboard</h2>
            <p>Welcome to your fitness dashboard!</p>
            <section>
                <h3>Profile</h3>
                <div>
                    <span>Name:{currentUser?.clientProfile?.firstName} {currentUser?.clientProfile?.lastName}</span><br />
                    <span>Age:{currentUser?.clientProfile?.age}</span><br />
                    <span>Email: {currentUser?.email}</span>
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
};

export default ClientDashboard;