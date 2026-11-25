import { useEffect, useState } from "react";

import {
    FaUsers,
    FaSearch,
    FaExclamationCircle,
    FaSyncAlt
} from "react-icons/fa";

import discoverService from "../services/discoverService";

import DiscoverCard from "../components/discover/DiscoverCard";
import DiscoverFilters from "../components/discover/DiscoverFilters";
import DiscoverSkeleton from "../components/discover/DiscoverSkeleton";
import DiscoverProfileModal from "../components/discover/DiscoverProfileModal";
import Pagination from "../components/discover/Pagination";
import DiscoverAdvancedFilters
    from "../components/discover/DiscoverAdvancedFilters";

import { FaSlidersH } from "react-icons/fa";
import "./Discover.css";

const PAGE_SIZE = 6;
const DEFAULT_ADVANCED_FILTERS = {
    category: "",
    interestIds: [],
    matchAllInterests: false
};
export default function Discover() {

    const [search, setSearch] = useState("");

    const [debouncedSearch, setDebouncedSearch] =
        useState("");

    const [sortBy, setSortBy] =
        useState("newest");

    const [pageNumber, setPageNumber] =
        useState(1);

    const [profiles, setProfiles] =
        useState([]);

    const [totalCount, setTotalCount] =
        useState(0);

    const [totalPages, setTotalPages] =
        useState(0);

    const [loading, setLoading] =
        useState(true);

    const [error, setError] =
        useState("");

    const [selectedProfileId, setSelectedProfileId] =
        useState(null);

    const [reloadKey, setReloadKey] =
        useState(0);

    const [advancedFiltersOpen, setAdvancedFiltersOpen] =
        useState(false);

    const [advancedFilters, setAdvancedFilters] =
        useState(DEFAULT_ADVANCED_FILTERS);
    useEffect(() => {
        console.log("Current filters:", advancedFilters);
    }, [advancedFilters]);
    // Debounce Search
    useEffect(() => {
        const timer = setTimeout(() => {
            setDebouncedSearch(search.trim());
            setPageNumber(1);
        }, 400);

        return () => clearTimeout(timer);
    }, [search]);

    // Fetch profiles
    useEffect(() => {
        const controller = new AbortController();

        const fetchProfiles = async () => {
            try {
                setLoading(true);
                setError("");

                const data = await discoverService.getProfiles(
                    {
                        search: debouncedSearch,
                        sortBy,
                        pageNumber,
                        pageSize: PAGE_SIZE,
                        category: advancedFilters.category,
                        interestIds: advancedFilters.interestIds,
                        matchAllInterests:
                            advancedFilters.matchAllInterests
                    },
                    controller.signal
                );

                if (controller.signal.aborted) {
                    return;
                }

                const pages = data.totalPages ?? 0;

                if (pages > 0 && pageNumber > pages) {
                    setPageNumber(pages);
                    return;
                }

                setProfiles(data.items ?? []);
                setTotalCount(data.totalCount ?? 0);
                setTotalPages(pages);
            } catch (err) {
                if (controller.signal.aborted) {
                    return;
                }

                const status = err.response?.status;

                if (status === 403) {
                    setError(
                        "Administrator permission is required."
                    );
                } else if (status === 401) {
                    setError(
                        "Your session has expired."
                    );
                } else {
                    setError(
                        "Unable to load profiles. Please try again."
                    );
                }

                setProfiles([]);
                setTotalCount(0);
                setTotalPages(0);
            } finally {
                if (!controller.signal.aborted) {
                    setLoading(false);
                }
            }
        };

        fetchProfiles();

        return () => controller.abort();
    }, [
        debouncedSearch,
        sortBy,
        pageNumber,
        reloadKey,
        advancedFilters
    ]);

    const handleSortChange = (value) => {
        setSortBy(value);
        setPageNumber(1);
    };

    const handlePageChange = (page) => {
        if (page < 1 || page > totalPages) {
            return;
        }

        setPageNumber(page);
    };

    const handleRetry = () => {
        setReloadKey((previous) => previous + 1);
    };

    const handleClearFilters = () => {
        setSearch("");
        setDebouncedSearch("");
        setSortBy("newest");
        setPageNumber(1);

        setAdvancedFilters({
            ...DEFAULT_ADVANCED_FILTERS,
            interestIds: []
        });
    };
    const handleApplyAdvancedFilters = (newFilters) => {
        setAdvancedFilters({
            category: newFilters.category || "",
            interestIds: [...(newFilters.interestIds || [])],
            matchAllInterests:
                newFilters.matchAllInterests || false
        });

        setSortBy(newFilters.sortBy || "newest");

        setPageNumber(1);

        setAdvancedFiltersOpen(false);
    };
    const activeFilterCount =
        advancedFilters.interestIds.length +
        (advancedFilters.category ? 1 : 0);
    
    return (
        <div className="discover-page">

            {/* Page heading */}
            <div className="discover-heading">
                <div>
                    <h1>Discover</h1>

                    <p>
                        Browse and manage database profiles.
                    </p>
                </div>

                <div className="discover-heading-icon">
                    <FaUsers />
                </div>
            </div>

            <div className="discover-toolbar">
                {/* Filters */}
                <DiscoverFilters
                    search={search}
                    sortBy={sortBy}
                    onSearchChange={setSearch}
                    onSortChange={handleSortChange}

                />
                <button
                    type="button"
                    className="discover-advanced-filter-btn"
                    onClick={() => setAdvancedFiltersOpen(true)}
                >
                    <FaSlidersH />

                    Filters

                    {activeFilterCount > 0 && (
                        <span className="discover-filter-count">
                            {activeFilterCount}
                        </span>
                    )}
                </button>
            </div>

            {/* Result count */}
            {!loading && !error && (
                <div className="discover-result-count">
                    {totalCount} profiles found
                </div>
            )}

            {/* Loading */}
            {loading ? (
                <DiscoverSkeleton count={PAGE_SIZE} />

                /* Error */
            ) : error ? (
                <div className="discover-state">
                    <FaExclamationCircle
                        className="discover-state-icon"
                    />

                    <h3>Something went wrong</h3>

                    <p>{error}</p>

                    <button
                        className="discover-primary-btn"
                        onClick={handleRetry}
                    >
                        <FaSyncAlt />
                        Try again
                    </button>
                </div>

                /* Empty state */
            ) : profiles.length === 0 ? (
                <div className="discover-state">
                    <FaSearch
                        className="discover-state-icon"
                    />

                    <h3>No profiles found</h3>

                    <p>
                        Try changing your search or filters.
                    </p>

                    <button
                        className="discover-primary-btn"
                        onClick={handleClearFilters}
                    >
                        Clear filters
                    </button>
                </div>

                /* Profile list */
            ) : (
                <>
                    <div className="discover-grid">
                        {profiles.map((profile) => (
                            <DiscoverCard
                                key={profile.id}
                                profile={profile}
                                onViewProfile={
                                    setSelectedProfileId
                                }
                            />
                        ))}
                    </div>

                    <Pagination
                        pageNumber={pageNumber}
                        totalPages={totalPages}
                        onPageChange={handlePageChange}
                    />
                </>
            )}

            {/* Profile details modal */}
            {selectedProfileId !== null && (
                <DiscoverProfileModal
                    profileId={selectedProfileId}
                    onClose={() =>
                        setSelectedProfileId(null)
                    }
                />
            )}
            {advancedFiltersOpen && (
                <DiscoverAdvancedFilters
                    filters={{
                        ...advancedFilters,
                        sortBy
                    }}
                    onClose={() =>
                        setAdvancedFiltersOpen(false)
                    }
                    onApply={handleApplyAdvancedFilters}
                />
            )}
        </div>
    );
}