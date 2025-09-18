import React, { useState } from "react";
import TableGeneric from "../../components/TableGeneric";
import Dropdown from "../../components/Dropdown";
import Button from "../../components/Button";
import GenericFormDialog from "../../components/GenericFormDialog";
import type { CreateUserDto, UserStatisticsDto } from "../../utils/data/userdtos";
import useAdminDashboard from "../../hooks/useAdminDashboard";
import { useAuth } from "../../utils/contexts/AuthContext";
import { adminFetchUserStatistics } from "../../utils/api/api";

const AdminDashboard: React.FC = () => {
    // Use the custom hook
    const {
        users,
        loading,
        error,
        setError,
        page,
        setPage,
        pageSize,
        setPageSize,
        totalPages,
        sortBy,
        sortOrder,
        handleSort,
        handleCreateUser,
        handleDeleteUser,
        PAGE_SIZES
    } = useAdminDashboard();

    const { token, currentUser } = useAuth(); // Needed for selectedUserId logic
    const [openCreateDialog, setOpenCreateDialog] = useState(false);
    const [selectedUserId, setSelectedUserId] = useState<number | null>(null);
    const [stats, setStats] = useState<UserStatisticsDto | null>(null);

    return (
        <div>
            <h2>Admin Dashboard</h2>
            <section>
                <h3>User Management</h3>
                <div style={{ display: "flex", alignItems: "center", gap: "1rem" }}>
                    <Dropdown
                        label="Page Size"
                        name="pageSize"
                        value={pageSize.toString()}
                        options={PAGE_SIZES.map(s => ({ value: s.toString(), label: s.toString() }))}
                        onChange={e => { setPageSize(Number(e.target.value)); setPage(1); }}
                    />

                    <Button onClick={() => setPage(page - 1)} disabled={page <= 1}>
                        <i className="fas fa-chevron-left" aria-label="Previous page"></i>
                    </Button>
                    <span>Page {page} of {totalPages}</span>
                    <Button onClick={() => setPage(page + 1)} disabled={page >= totalPages}>
                        <i className="fas fa-chevron-right" aria-label="Next page"></i>
                    </Button>
                    <Button onClick={() => setOpenCreateDialog(true)}>
                        <i className="fas fa-user-plus" aria-label="Add User"></i>
                    </Button>
                    <Button
                        onClick={async () => {
                            if (!selectedUserId || !token) return;
                            if (currentUser && selectedUserId === currentUser.id) {
                                setError("You cannot delete your own account.");
                                setTimeout(() => setError(null), 1000); // Clear after 1s
                                return;
                            }
                            await handleDeleteUser(selectedUserId);
                            setSelectedUserId(null);
                        }}
                        disabled={!selectedUserId}
                    >
                        <i className="fas fa-user-minus" aria-label="Delete User"></i>
                    </Button>

                    <GenericFormDialog<CreateUserDto>
                        open={openCreateDialog}
                        onClose={() => setOpenCreateDialog(false)}
                        onSubmit={async (data) => {
                            await handleCreateUser(data);
                            setOpenCreateDialog(false);
                        }}
                        title="Create New User"
                        initialValues={{ username: "", email: "", password: "", role: "Client" }}
                        fields={[
                            { name: "username", label: "Username", required: true },
                            { name: "email", label: "Email", type: "email", required: true },
                            { name: "password", label: "Password", type: "password", required: true },
                            {
                                name: "role",
                                label: "Role",
                                type: "dropdown",
                                required: true,
                                options: [
                                    { value: "Client", label: "Client" },
                                    { value: "Trainer", label: "Trainer" },
                                    { value: "Admin", label: "Admin" }
                                ]
                            }
                        ]}
                    />

                </div>
                {loading ? (
                    <div>Loading...</div>
                ) : (
                    <TableGeneric
                        data={users.map(u => ({
                            id: u.id,
                            createdUtc: u.createdUtcFormatted || u.createdUtc,
                            profilePhotoUrl: u.profilePhotoUrl,
                            username: u.username,
                            email: u.email,
                            isActive: u.isActive,
                            role: u.role,
                            trainerProfile: u.trainerProfile,
                            clientProfile: u.clientProfile
                        }))}
                        onSort={handleSort}
                        sortBy={sortBy}
                        sortOrder={sortOrder}
                        selectable
                        selectedRowId={selectedUserId}
                        onRowSelect={row => setSelectedUserId(row?.id ?? null)}
                    />
                )}
                {error && (
                    <div style={{ color: "red", marginTop: "1rem" }}>{error}</div>
                )}
            </section>
            <section>
                <h3>User Statistics</h3>
                {/*Display user statistics here in span elements, call the api once upon clicking the button*/}
                <Button
                    onClick={async () => {
                        if (!token) return;
                        const data = await adminFetchUserStatistics(token);
                        setStats(data);
                    }}
                >
                    Fetch User Statistics
                </Button>
                <div id="user-statistics" style={{ marginTop: "1rem" }}>
                    <span>Total Users: {stats?.totalUsers ?? 0}</span><br/>
                    <span>Active Users: {stats?.activeUsers ?? 0}</span><br/>
                    <span>Inactive Users: {stats?.inactiveUsers ?? 0}</span><br/>
                    <span>Admins: {stats?.admins ?? 0}</span><br/>
                    <span>Trainers: {stats?.trainers ?? 0}</span><br/>
                    <span>Clients: {stats?.clients ?? 0}</span><br/>
                </div>
            </section>
        </div>
    );
};

export default AdminDashboard;