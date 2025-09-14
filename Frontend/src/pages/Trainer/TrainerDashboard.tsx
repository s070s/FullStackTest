import React from 'react';

const TrainerDashboard: React.FC = () => {
    return (
        <div>
            <h2>Trainer Dashboard</h2>
            <p>Welcome, trainer! Manage your clients and workouts here.</p>
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
};

export default TrainerDashboard;