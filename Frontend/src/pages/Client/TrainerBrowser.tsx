import React, { useEffect, useState } from "react";
import TableGeneric from "../../components/TableGeneric";
import Dropdown from "../../components/Dropdown";
import Button from "../../components/Button";
import type { TrainerDto } from "../../utils/data/trainerdtos";
import LoadingSpinner from "../../components/LoadingSpinner";
import { useAuth } from "../../utils/contexts/AuthContext";
import { useNavigate } from "react-router-dom";
import { readAllTrainersPaginated } from "../../utils/api/api";



//Todo:Search the User of a Trainer to get the photo from or Refactor Models
//add photos to PersonalInformation Class on the backend
const TrainerBrowser: React.FC = () => {
    const navigate = useNavigate();
    const [trainers, setTrainers] = useState<TrainerDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(5);
    const PAGE_SIZES = [5, 10, 50, 100];
    const [sortBy, setSortBy] = useState<string>("id");
    const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");
    const [totalUsers, setTotalUsers] = useState<number>(0);
    const { ensureAccessToken} = useAuth();






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
                    data={trainers.map(u => ({
                        id: u.id,
                        profilePhotoUrl:u.profilePhotoUrl,
                        fullName: `${u.firstName} ${u.lastName}`,
                    }))}
                    onSort={handleSort}
                    sortBy={sortBy}
                    sortOrder={sortOrder}
                />
            )}
            {error && (
                <div style={{ color: "red", marginTop: "1rem" }}>{error}</div>
            )}
            <Button onClick={() => navigate("/dashboard")}>
                Back to Dashboard
            </Button>
        </div>
    );
};

export default TrainerBrowser;