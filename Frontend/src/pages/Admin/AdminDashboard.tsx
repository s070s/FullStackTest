import React, { useEffect, useState } from "react";
import TableGeneric from "../../components/TableGeneric";
import Dropdown from "../../components/Dropdown";
import { adminFetchAllUsers } from "../../utils/api/api";
import type { UserDto } from "../../utils/data/userdtos";
import { useAuth } from "../../utils/contexts/AuthContext";


const AdminDashboard: React.FC = () => {

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
    const { token } = useAuth();

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

    const handleSort = (col: string) => {
        if (sortBy === col) {
            setSortOrder(sortOrder === "asc" ? "desc" : "asc");
        } else {
            setSortBy(col);
            setSortOrder("asc");
        }
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
                    <button disabled={page <= 1} onClick={() => setPage(page - 1)}>{"<"}</button>
                    <span>Page {page} of {totalPages}</span>
                    <button disabled={page >= totalPages} onClick={() => setPage(page + 1)}>{">"}</button>
                </div>
                {loading ? (
                    <div>Loading...</div>
                ) : error ? (
                    <div>Error: {error}</div>
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
                    />
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