import { useEffect, useState } from "react";
import type { CreateUserDto, UserDto, UpdateUserDto } from "../utils/data/userdtos";
import { adminFetchAllUsers, adminDeleteUser, adminCreateUser, adminUpdateUser } from "../utils/api/api";
import { useAuth } from "../utils/contexts/AuthContext";

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
    const { token, currentUser } = useAuth();

    useEffect(() => {
        if (token) {
            adminFetchAllUsers(token, { page, pageSize, sortBy, sortOrder })
                .then((data: { users: UserDto[]; total: number }) => {
                    setUsers(data.users);
                    setTotalUsers(data.total);
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

    const handleCreateUser = async (data: CreateUserDto) => {
        if (!token) return;
        await adminCreateUser(token, data);
        await refreshUsers();
    };

    const handleDeleteUser = async (selectedUserId: number | null) => {
        if (!selectedUserId || !token) return;
        if (currentUser && selectedUserId === currentUser.id) {
            setError("You cannot delete your own account.");
            return;
        }
        await adminDeleteUser(token, selectedUserId);
        await refreshUsers();
    };

    const handleUpdateUser = async (userId: number, data: UpdateUserDto) => {
        if (!token) return;

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
        await adminUpdateUser(token, userId, fixedData);
        await refreshUsers();
    };

    const refreshUsers = async () => {
        if (!token) return;
        const data = await adminFetchAllUsers(token, { page, pageSize, sortBy, sortOrder });
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