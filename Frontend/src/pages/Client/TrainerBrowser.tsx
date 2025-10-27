import React, { useEffect, useState, useMemo, useCallback } from "react";
import TableGeneric from "../../components/TableGeneric";
import Dropdown from "../../components/Dropdown";
import Button from "../../components/Button";
import type { TrainerDto } from "../../utils/data/trainerdtos";
import LoadingSpinner from "../../components/LoadingSpinner";
import { useAuth } from "../../utils/contexts/AuthContext";
import { useNavigate } from "react-router-dom";
import { readAllTrainersPaginated, subscribeToTrainer, unsubscribeFromTrainer, getSubscribedTrainerIds } from "../../utils/api/api";

const TrainerBrowser: React.FC = () => {
    const navigate = useNavigate();
    const [trainers, setTrainers] = useState<TrainerDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null); // Success message state
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(5);
    const PAGE_SIZES = [5, 10, 50, 100];
    const [sortBy, setSortBy] = useState<string>("id");
    const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");
    const [totalUsers, setTotalUsers] = useState<number>(0);
    const [subscribingTrainerId, setSubscribingTrainerId] = useState<number | null>(null);
    const [subscribedTrainerIds, setSubscribedTrainerIds] = useState<number[]>([]); // Track subscribed trainers
    const { ensureAccessToken, currentUser } = useAuth();

    useEffect(() => {
        let isMounted = true;

        const fetchTrainers = async () => {
            setLoading(true);
            try {
                const accessToken = await ensureAccessToken();
                if (!accessToken) {
                    if (isMounted) {
                        setError("No authentication token found.");
                    }
                    return;
                }

                const data = await readAllTrainersPaginated(accessToken, page, pageSize, sortBy, sortOrder);
                if (isMounted) {
                    setTrainers(data.trainers);
                    setTotalUsers(data.total);
                    setError(null);
                }
            } catch (err: any) {
                if (isMounted) {
                    setError(err.message ?? "Failed to fetch trainers.");
                }
            } finally {
                if (isMounted) {
                    setLoading(false);
                }
            }
        };

        fetchTrainers();

        return () => {
            isMounted = false;
        };
    }, [ensureAccessToken, page, pageSize, sortBy, sortOrder]);

    useEffect(() => {
        let isMounted = true;
        const fetchSubscriptions = async () => {
            if (!currentUser) return;
            try {
                const accessToken = await ensureAccessToken();
                if (!accessToken) return;
                const ids = await getSubscribedTrainerIds(accessToken, currentUser.id);
                if (isMounted) setSubscribedTrainerIds(ids);
            } catch (err: any) {
                if (isMounted) setError(err?.message ?? "Failed to fetch subscriptions.");
            }
        };
        fetchSubscriptions();
        return () => { isMounted = false; };
    }, [currentUser, ensureAccessToken]);

    const totalPages = Math.ceil(totalUsers / pageSize);


    const validSortKeys = ["id", "firstName", "lastName"];

    // memoize sort handler to avoid new function each render
    const handleSort = useCallback((col: string) => {
        if (!validSortKeys.includes(col)) return; // Ignore invalid sort keys
        if (sortBy === col) {
            setSortOrder(prevOrder => (prevOrder === "asc" ? "desc" : "asc"));
        } else {
            setSortBy(col);
            setSortOrder("asc");
        }
    }, [sortBy]);

    // memoize mapped table data to avoid recreating array each render
    const tableData = useMemo(() => {
        return trainers.map(u => ({
            id: u.id,
            profilePhotoUrl: u.profilePhotoUrl,
            firstName: u.firstName,
            lastName: u.lastName
        }));
    }, [trainers]);

    // memoize subscription toggle handler
    const handleToggleSubscribe = useCallback(async (trainerId: number) => {
        if (!currentUser) {
            setError("You must be logged in.");
            return;
        }
        setSubscribingTrainerId(trainerId);
        setError(null);
        setSuccess(null);
        try {
            const accessToken = await ensureAccessToken();
            if (!accessToken) {
                setError("No authentication token found.");
                return;
            }
            if (subscribedTrainerIds.includes(trainerId)) {
                await unsubscribeFromTrainer(accessToken, currentUser.id, trainerId);
                setSuccess("Unsubscribed from trainer successfully!");
            } else {
                await subscribeToTrainer(accessToken, currentUser.id, trainerId);
                setSuccess("Subscribed to trainer successfully!");
            }
            // refresh subscriptions
            const ids = await getSubscribedTrainerIds(accessToken, currentUser.id);
            setSubscribedTrainerIds(ids);
            setError(null);
        } catch (err: any) {
            setError(err.message ?? "Failed to update subscription.");
        } finally {
            setSubscribingTrainerId(null);
        }
    }, [ensureAccessToken, currentUser, subscribedTrainerIds]);

    // memoize renderCell so TableGeneric receives a stable callback
    const renderCell = useCallback((col: string, row: { id?: number; profilePhotoUrl?: string; fullName?: string }) => {
        if (col === "__actions") {
            const isSubscribed = row.id ? subscribedTrainerIds.includes(row.id) : false;
            return (
                <Button
                    onClick={() => row.id && handleToggleSubscribe(row.id)}
                    disabled={subscribingTrainerId === row.id}
                    className={isSubscribed ? "success-button" : undefined}
                >
                    {subscribingTrainerId === row.id
                        ? <LoadingSpinner />
                        : isSubscribed
                            ? (<><i className="fas fa-check" style={{ marginRight: 6 }}></i>Subscribed</>)
                            : "Subscribe"}
                </Button>
            );
        }
        return undefined;
    }, [subscribedTrainerIds, subscribingTrainerId, handleToggleSubscribe]);

    const columns = [
        { key: "id", label: "Id" },
        { key: "profilePhotoUrl", label: "Photo" },
        { key: "firstName", label: "First Name" },
        { key: "lastName", label: "Last Name" }
        // Add other columns as needed
    ];

    return (
        <div>
            <h2>Find Trainers</h2>
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
            </div>
            {loading ? (
                <LoadingSpinner />
            ) : (
                <TableGeneric
                    data={tableData}
                    columns={columns}
                    onSort={handleSort}
                    sortBy={sortBy}
                    sortOrder={sortOrder}
                    renderCell={renderCell}
                />
            )}
            {error && (
                <div style={{ color: "red", marginTop: "1rem" }}>{error}</div>
            )}
            {success && (
                <div style={{ color: "green", marginTop: "1rem" }}>{success}</div>
            )}
            <Button onClick={() => navigate("/dashboard")}>
                Back to Dashboard
            </Button>
        </div>
    );
};

export default TrainerBrowser;