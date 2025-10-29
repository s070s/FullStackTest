import React, { useState, useMemo, useCallback } from "react";
import TableGeneric from "../../components/TableGeneric";
import Dropdown from "../../components/Dropdown";
import Button from "../../components/Button";
import GenericFormDialog from "../../components/GenericFormDialog";
import type { CreateUserDto, UserStatisticsDto, UserDto, UpdateUserDto } from "../../utils/data/userdtos";
import useAdminDashboard from "./useAdminDashboard";
import LoadingSpinner from "../../components/LoadingSpinner";
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
        handleUpdateUser,
        handleDeleteUser,
        PAGE_SIZES
    } = useAdminDashboard();

    const { currentUser, ensureAccessToken } = useAuth(); // Needed for selectedUserId logic
    const [openCreateDialog, setOpenCreateDialog] = useState(false);
    const [selectedUserId, setSelectedUserId] = useState<number | null>(null);
    const [stats, setStats] = useState<UserStatisticsDto | null>(null);
    const [editUser, setEditUser] = useState<UserDto | null>(null);
    const [statsLoading, setStatsLoading] = useState(false);

    // memoize table data so TableGeneric doesn't receive a new array each render
    const tableData = useMemo(() => users.map(u => ({
        id: u.id,
        profilePhotoUrl:
            u.role === "Client" && u.clientProfile?.profilePhotoUrl
                ? u.clientProfile.profilePhotoUrl
                : u.role === "Trainer" && u.trainerProfile?.profilePhotoUrl
                    ? u.trainerProfile.profilePhotoUrl
                    : undefined,
        createdUtc: u.createdUtcFormatted || u.createdUtc,
        username: u.username,
        email: u.email,
        isActive: u.isActive,
        role: u.role,
        trainerProfile: u.trainerProfile,
        clientProfile: u.clientProfile
    })), [users]);

    // memoize renderCell to keep stable function identity
    const renderCell = useCallback((col: string, row: any) =>
        col === "username" ? (
            <a
                href="#"
                style={{
                    color: "blue",
                    textDecoration: "underline",
                    cursor: "pointer"
                }}
                onClick={e => {
                    e.preventDefault();
                    const user = users.find(u => u.id === row.id);
                    if (user) setEditUser(user);
                }}
            >
                {row.username}
            </a>
        ) : undefined
    , [users, setEditUser]);

    return (
        <div>
            <h2>Admin Dashboard</h2>
            <section>
                <h3>User Management</h3>
                <div style={{ display: "flex", alignItems: "center", gap: "1rem" }}>
                    {/** Pager Page Size*/}
                    <Dropdown
                        label="Page Size"
                        name="pageSize"
                        value={pageSize.toString()}
                        options={PAGE_SIZES.map(s => ({ value: s.toString(), label: s.toString() }))}
                        onChange={e => { setPageSize(Number(e.target.value)); setPage(1); }}
                    />
                    {/** Page Navigation Controls */}
                    <Button onClick={() => setPage(page - 1)} disabled={page <= 1}>
                        <i className="fas fa-chevron-left" aria-label="Previous page"></i>
                    </Button>
                    <span>Page {page} of {totalPages}</span>
                    <Button onClick={() => setPage(page + 1)} disabled={page >= totalPages}>
                        <i className="fas fa-chevron-right" aria-label="Next page"></i>
                    </Button>
                    {/** Create and Delete User Buttons */}
                    <Button onClick={() => setOpenCreateDialog(true)}>
                        <i className="fas fa-user-plus" aria-label="Add User"></i>
                    </Button>
                    <Button
                        onClick={async () => {
                            if (!selectedUserId) return;
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
                    {/** Create User Button */}
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
                {/** Users Table */}
                {loading ? (
                    <LoadingSpinner />
                ) : (
                    <TableGeneric
                        data={tableData}
                        onSort={handleSort}
                        sortBy={sortBy}
                        sortOrder={sortOrder}
                        selectable
                        selectedRowId={selectedUserId}
                        onRowSelect={row => setSelectedUserId(row?.id ?? null)}
                        //render user names as links that open the edit dialog
                        renderCell={renderCell}
                    />
                )}
                {error && (
                    <div style={{ color: "red", marginTop: "1rem" }}>{error}</div>
                )}
                {/** Edit User Dialog */}
                <GenericFormDialog<UpdateUserDto>
                    open={!!editUser}
                    onClose={() => setEditUser(null)}
                    onSubmit={async (data) => {
                        if (!editUser) return;
                        await handleUpdateUser(editUser.id, data);
                        setEditUser(null);
                    }}
                    title={`Edit User: ${editUser?.username ?? ""}`}
                    initialValues={{
                        username: editUser?.username ?? "",
                        email: editUser?.email ?? "",
                        password: "",
                        isActive: editUser?.isActive ?? true,
                        role: editUser?.role ?? "Client"
                    }}
                    fields={[
                        { name: "username", label: "Username", required: false },
                        { name: "email", label: "Email", type: "email", required: false },
                        { name: "password", label: "Password", type: "password", required: false },
                        { name: "isActive", label: "Active", type: "checkbox", required: false },
                        {
                            name: "role",
                            label: "Role",
                            type: "dropdown",
                            required: false,
                            options: [
                                { value: "Client", label: "Client" },
                                { value: "Trainer", label: "Trainer" },
                                { value: "Admin", label: "Admin" }
                            ]
                        }
                    ]}
                />
            </section>
            <section>
                <h3>User Statistics</h3>
                {/* call the endpoint that fetches user statistics once upon clicking the button */}
                <Button
                    onClick={async () => {
                        setStatsLoading(true); // Start spinner
                        try {
                            const accessToken = await ensureAccessToken();
                            if (!accessToken) return;
                            const data = await adminFetchUserStatistics(accessToken);
                            setStats(data);
                        } finally {
                            setStatsLoading(false); // Stop spinner
                        }
                    }}
                >
                    Fetch User Statistics
                </Button>
                {/** Display user statistics or loading spinner */}
                <div id="user-statistics" style={{ marginTop: "1rem" }}>
                    {statsLoading ? (
                        <LoadingSpinner />
                    ) : (
                        <>
                            <span>Total Users: {stats?.totalUsers ?? 0}</span><br />
                            <span>Active Users: {stats?.activeUsers ?? 0}</span><br />
                            <span>Inactive Users: {stats?.inactiveUsers ?? 0}</span><br />
                            <span>Admins: {stats?.admins ?? 0}</span><br />
                            <span>Trainers: {stats?.trainers ?? 0}</span><br />
                            <span>Clients: {stats?.clients ?? 0}</span><br />
                        </>
                    )}
                </div>
            </section>
        </div>
    );
};

export default AdminDashboard;