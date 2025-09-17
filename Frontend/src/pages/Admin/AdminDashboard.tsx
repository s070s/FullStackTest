import React, { useEffect, useState } from "react";
import TableGeneric from "../../components/TableGeneric";
import Dropdown from "../../components/Dropdown";
import Button from "../../components/Button";
import GenericFormDialog from "../../components/GenericFormDialog";
import type { CreateUserDto } from "../../utils/data/userdtos";
import type { UserDto } from "../../utils/data/userdtos";
import { adminFetchAllUsers, adminDeleteUser, adminCreateUser } from "../../utils/api/api";
import { useAuth } from "../../utils/contexts/AuthContext";


const AdminDashboard: React.FC = () => {

    // State for users and UI
    const [users, setUsers] = useState<UserDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    // Pagination and sorting state
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(5);
    const PAGE_SIZES = [5, 10, 50, 100];
    const [sortBy, setSortBy] = useState<string>("id");
    const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");
    const [totalUsers, setTotalUsers] = useState<number>(0);
    // Authentication context
    const { token, currentUser } = useAuth();
    //Create User Dialog
    const [openCreateDialog, setOpenCreateDialog] = useState(false);
    //Selected User from the table
    const [selectedUserId, setSelectedUserId] = useState<number | null>(null);



    useEffect(() => {
        if (token) {
            adminFetchAllUsers(token, { page, pageSize, sortBy, sortOrder })
                .then((data: { users: UserDto[]; total: number }) => { // <-- add type here
                    console.log("API response:", data);
                    setUsers(data.users);      // <-- use data.users
                    setTotalUsers(data.total); // <-- use data.total
                })
                .catch((err) => setError(err.message))
                .finally(() => setLoading(false));
        } else {
            setError("No authentication token found.");
            setLoading(false);
        }
    }, [token, page, pageSize, sortBy, sortOrder]);

    const totalPages = Math.ceil(totalUsers / pageSize);
    // Handler for sorting
    const handleSort = (col: string) => {
        if (sortBy === col) {
            setSortOrder(sortOrder === "asc" ? "desc" : "asc");
        } else {
            setSortBy(col);
            setSortOrder("asc");
        }
    };

    // Handler for creating a user
    const handleCreateUser = async (data: CreateUserDto) => {
        if (!token) return;
        await adminCreateUser(token, data);
        // Refresh users after creation
        adminFetchAllUsers(token, { page, pageSize, sortBy, sortOrder })
            .then((data: { users: UserDto[]; total: number }) => {
                setUsers(data.users);
                setTotalUsers(data.total);
            });
        setOpenCreateDialog(false);
    };

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
                                setTimeout(() => {
                                    setError(null);
                                    adminFetchAllUsers(token, { page, pageSize, sortBy, sortOrder })
                                        .then((data: { users: UserDto[]; total: number }) => {
                                            setUsers(data.users);
                                            setTotalUsers(data.total);
                                        });
                                }, 1000);
                                return;
                            }
                            await adminDeleteUser(token, selectedUserId);
                            // Refresh users after deletion
                            adminFetchAllUsers(token, { page, pageSize, sortBy, sortOrder })
                                .then((data: { users: UserDto[]; total: number }) => {
                                    setUsers(data.users);
                                    setTotalUsers(data.total);
                                });
                            setSelectedUserId(null);
                        }}
                        disabled={!selectedUserId}
                    >
                        <i className="fas fa-user-minus" aria-label="Delete User"></i>
                    </Button>


                    <GenericFormDialog<CreateUserDto>
                        open={openCreateDialog}
                        onClose={() => setOpenCreateDialog(false)}
                        onSubmit={handleCreateUser}
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
                <h3>System Statistics</h3>
                <div>
                    <span>Total Users: {totalUsers}</span><br />
                </div>
            </section>
            <section>
                <h3>Settings</h3>
                <div>
                    <span>System Status: [Online]</span>
                </div>
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

export default AdminDashboard;