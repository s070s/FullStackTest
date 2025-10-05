import React from 'react';
import { useState } from 'react';
import { useAuth } from "../../utils/contexts/AuthContext";
import Button from "../../components/Button";
import GenericFormDialog from "../../components/GenericFormDialog";
import { updateUserProfile } from "../../utils/api/api";
import type { ClientUpdateDto } from "../../utils/data/clientdtos";



const ClientDashboard: React.FC = () => {
    const { currentUser, token, refreshUser } = useAuth();
    const [dialogOpen, setDialogOpen] = useState(false);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);




    const profileFields: { name: keyof ClientUpdateDto; label?: string; type?: string; required?: boolean }[] = [
        { name: "firstName", label: "First Name", required: true },
        { name: "lastName", label: "Last Name", required: true },
        { name: "bio", label: "Bio" },
        { name: "dateOfBirth", label: "Date of Birth", type: "date" },
        { name: "height", label: "Height (cm)", type: "number" },
        { name: "weight", label: "Weight (kg)", type: "number" },
        { name: "phoneNumber", label: "Phone Number" },
        { name: "country", label: "Country" },
        { name: "city", label: "City" },
        { name: "address", label: "Address" },
        { name: "zipCode", label: "Zip Code" },
        { name: "state", label: "State" },
        { name: "experienceLevel", label: "Experience Level" },
    ];
    if (!currentUser?.clientProfile) {
        return <div>Loading profile...</div>;
    }
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
                <Button
                    onClick={() => {
                        if (currentUser?.clientProfile) setDialogOpen(true);
                        else setError("Profile data not loaded.");
                    }}
                >
                    Change Profile Settings
                </Button>
            </section>

            <GenericFormDialog<ClientUpdateDto>
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
                        await refreshUser(); // Add this line to refresh user data
                    } catch (e: any) {
                        setError(e.message || "Failed to update profile.");
                    } finally {
                        setLoading(false);
                    }
                }}
                initialValues={currentUser?.clientProfile || {}}
                fields={profileFields}
                title="Edit Profile"
            />
            {error && <div style={{ color: "red" }}>{error}</div>}
            {loading && <div>Updating...</div>}




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