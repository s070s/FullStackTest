import { useEffect, useState, useCallback } from "react";
import type { CreateUserDto, UserDto, UpdateUserDto } from "../../utils/data/userdtos";
import { adminFetchAllUsers, adminDeleteUser, adminCreateUser, adminUpdateUser } from "../../utils/api/api";
import { useAuth } from "../../utils/contexts/AuthContext";

const useAdminDashboard = () => {
    const [users, setUsers] = useState<UserDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(5);
    const PAGE_SIZES = [5, 10, 50, 100];
    const [sortBy, setSortBy] = useState<string>("id");
    const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");
    const [totalUsers, setTotalUsers] = useState<number>(0);
    const { ensureAccessToken, currentUser } = useAuth();

    useEffect(() => {
        let isMounted = true;

        const fetchUsers = async () => {
            setLoading(true);
            try {
                const accessToken = await ensureAccessToken();
                if (!accessToken) {
                    if (isMounted) {
                        setError("No authentication token found.");
                    }
                    return;
                }

                const data = await adminFetchAllUsers(accessToken, { page, pageSize, sortBy, sortOrder });
                if (isMounted) {
                    setUsers(data.users);
                    setTotalUsers(data.total);
                    setError(null);
                }
            } catch (err: any) {
                if (isMounted) {
                    setError(err.message ?? "Failed to fetch users.");
                }
            } finally {
                if (isMounted) {
                    setLoading(false);
                }
            }
        };

        fetchUsers();

        return () => {
            isMounted = false;
        };
    }, [ensureAccessToken, page, pageSize, sortBy, sortOrder]);

    const totalPages = Math.ceil(totalUsers / pageSize);

    const validSortKeys = ["id", "username", "email", "role", "isActive", "createdUtc"];
    
    const handleSort = useCallback((col: string) => {
        if (!validSortKeys.includes(col)) return; // Ignore invalid sort keys
        if (sortBy === col) {
            setSortOrder(prevOrder => (prevOrder === "asc" ? "desc" : "asc"));
        } else {
            setSortBy(col);
            setSortOrder("asc");
        }
    }, [sortBy]);

    const handleCreateUser = async (data: CreateUserDto) => {
        const accessToken = await ensureAccessToken();
        if (!accessToken) return;
        await adminCreateUser(accessToken, data);
        await refreshUsersList();
    };

    const handleDeleteUser = async (selectedUserId: number | null) => {
        if (!selectedUserId) return;
        if (currentUser && selectedUserId === currentUser.id) {
            setError("You cannot delete your own account.");
            return;
        }
        const accessToken = await ensureAccessToken();
        if (!accessToken) return;
        await adminDeleteUser(accessToken, selectedUserId);
        await refreshUsersList();
    };

    const handleUpdateUser = async (userId: number, data: UpdateUserDto) => {
        const accessToken = await ensureAccessToken();
        if (!accessToken) return;

        const roleEnumMap: Record<string, number> = { Admin: 0, Client: 1, Trainer: 2 };
        let roleString = typeof data.role === "string" ? data.role.trim() : "Client";
        let fixedRoleKey = Object.keys(roleEnumMap).find(
            r => r.toLowerCase() === roleString.toLowerCase()
        ) || "Client";
        let fixedRole = roleEnumMap[fixedRoleKey];

        const fixedData: any = {
            Id: userId,
            Role: fixedRole
        };
        if (data.username !== undefined) fixedData.Username = data.username;
        if (data.email !== undefined) fixedData.Email = data.email;
        if (data.password !== undefined) fixedData.Password = data.password;
        if (data.isActive !== undefined) {
            fixedData.IsActive =
                typeof data.isActive === "string"
                    ? data.isActive === "true"
                    : !!data.isActive;
        }
        await adminUpdateUser(accessToken, userId, fixedData);
        await refreshUsersList();
    };

    const refreshUsersList = async () => {
        const accessToken = await ensureAccessToken();
        if (!accessToken) return;
        const data = await adminFetchAllUsers(accessToken, { page, pageSize, sortBy, sortOrder });
        setUsers(data.users);
        setTotalUsers(data.total);
    };

    return {
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
        handleUpdateUser,
        PAGE_SIZES
    };
};

export default useAdminDashboard;