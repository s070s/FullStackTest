import React from 'react';
import { useState } from "react";
import Button from "../../components/Button";
import GenericFormDialog from "../../components/GenericFormDialog";
import { updateUserProfile } from "../../utils/api/api";
import type { TrainerUpdateDto } from "../../utils/data/trainerdtos";
import { useAuth } from "../../utils/contexts/AuthContext";


const TrainerDashboard: React.FC = () => {
    const { currentUser, token } = useAuth();

    const [dialogOpen, setDialogOpen] = useState(false);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const profileFields: Array<{
        name: keyof TrainerUpdateDto;
        label: string;
        type?: string;
        required?: boolean;
    }> = [
            { name: "firstName", label: "First Name", required: true },
            { name: "lastName", label: "Last Name", required: true },
            { name: "bio", label: "Bio" },
            { name: "dateOfBirth", label: "Date of Birth", type: "date" },
            { name: "phoneNumber", label: "Phone Number" },
            { name: "country", label: "Country" },
            { name: "city", label: "City" },
            { name: "address", label: "Address" },
            { name: "zipCode", label: "Zip Code" },
            { name: "state", label: "State" },
            // Add more fields as needed from TrainerUpdateDto
        ];
    if (!currentUser?.trainerProfile) {
        return <div>Loading profile...</div>;
    }
    return (
        <div>
            <h2>Trainer Dashboard</h2>
            <p>Welcome, {currentUser?.trainerProfile?.firstName} {currentUser?.trainerProfile?.lastName}! Manage your clients and workouts here.</p>
            <Button onClick={() => setDialogOpen(true)}>Change Profile Settings</Button>
            <GenericFormDialog<TrainerUpdateDto>
                open={dialogOpen}
                onClose={() => { setDialogOpen(false); setError(null); }}
                onSubmit={async (values) => {
                    setLoading(true);
                    setError(null);
                    try {
                        if (!token) {
                            setError("Authentication token is missing.");
                            return;
                        }
                        if (currentUser?.id === undefined) {
                            setError("User ID is missing.");
                            return;
                        }
                        await updateUserProfile(
                            token,
                            currentUser.id,
                            values
                        );
                        setDialogOpen(false);
                        // Optionally refresh user data here
                    } catch (e: any) {
                        setError(e.message || "Failed to update profile.");
                    } finally {
                        setLoading(false);
                    }
                }}
                initialValues={currentUser?.trainerProfile || {}}
                fields={profileFields}
                title="Edit Profile"
            />
            {error && <div style={{ color: "red" }}>{error}</div>}
            {loading && <div>Updating...</div>}
            <section>
                <h3>Manage your Clients and Workouts here.</h3>
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