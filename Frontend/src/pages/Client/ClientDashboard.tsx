import React from 'react';

const ClientDashboard: React.FC = () => {
    return (
        <div>
            <h2>Client Dashboard</h2>
            <p>Welcome to your fitness dashboard!</p>
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
};

export default ClientDashboard;