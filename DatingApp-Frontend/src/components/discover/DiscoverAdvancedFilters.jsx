import { useEffect, useMemo, useState } from "react";

import {
    FaSlidersH,
    FaTimes,
    FaCheck,
    FaRedo,
    FaSpinner,
    FaSearch
} from "react-icons/fa";

import interestService from "../../services/interestService";
import InterestIcon from "../interests/InterestIcon";

import "./DiscoverAdvancedFilters.css";

const MAX_INTERESTS = 10;

const DEFAULT_FILTERS = {
    category: "",
    interestIds: [],
    matchAllInterests: false,
    sortBy: "newest"
};

export default function DiscoverAdvancedFilters({
    filters,
    onApply,
    onClose
}) {
    const [interests, setInterests] = useState([]);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [search, setSearch] = useState("");
    const [reloadKey, setReloadKey] = useState(0);

    const [draft, setDraft] = useState(() => ({
        ...DEFAULT_FILTERS,
        ...filters,
        interestIds: [...(filters?.interestIds || [])]
    }));

    // Load interest catalog from API
    useEffect(() => {
        const controller = new AbortController();

        const loadInterests = async () => {
            try {
                setLoading(true);
                setError("");

                const data =
                    await interestService.getAll(
                        controller.signal
                    );

                if (!controller.signal.aborted) {
                    setInterests(
                        Array.isArray(data) ? data : []
                    );
                }
            } catch (err) {
                if (!controller.signal.aborted) {
                    setError(
                        err.response?.data?.message ||
                        "Unable to load interests."
                    );
                }
            } finally {
                if (!controller.signal.aborted) {
                    setLoading(false);
                }
            }
        };

        loadInterests();

        return () => controller.abort();

    }, [reloadKey]);

    // Close with Escape
    useEffect(() => {
        const handleKeyDown = (event) => {
            if (event.key === "Escape") {
                onClose?.();
            }
        };

        window.addEventListener(
            "keydown",
            handleKeyDown
        );

        return () => {
            window.removeEventListener(
                "keydown",
                handleKeyDown
            );
        };
    }, [onClose]);

    // Generate categories dynamically
    const categories = useMemo(() => {
        return [
            ...new Set(
                interests
                    .map(item => item.category)
                    .filter(Boolean)
            )
        ].sort((a, b) => a.localeCompare(b));
    }, [interests]);

    // Filter interest catalog
    const visibleInterests = useMemo(() => {
        const keyword = search.trim().toLowerCase();

        return interests.filter(item => {
            const matchesCategory =
                !draft.category ||
                item.category === draft.category;

            const matchesSearch =
                item.name
                    .toLowerCase()
                    .includes(keyword);

            return matchesCategory && matchesSearch;
        });
    }, [interests, draft.category, search]);

    const selectedInterests = interests.filter(
        item => draft.interestIds.includes(item.id)
    );

    const toggleInterest = (id) => {
        setDraft(previous => {
            const exists =
                previous.interestIds.includes(id);

            if (exists) {
                return {
                    ...previous,
                    interestIds:
                        previous.interestIds.filter(
                            item => item !== id
                        )
                };
            }

            if (
                previous.interestIds.length >=
                MAX_INTERESTS
            ) {
                return previous;
            }

            return {
                ...previous,
                interestIds: [
                    ...previous.interestIds,
                    id
                ]
            };
        });
    };

    const handleReset = () => {
        setDraft({
            ...DEFAULT_FILTERS,
            interestIds: []
        });

        setSearch("");
    };

    const handleApply = () => {
        onApply?.({
            category: draft.category,
            interestIds: [...draft.interestIds],
            matchAllInterests:
                draft.matchAllInterests,
            sortBy: draft.sortBy
        });
    };

    return (
        <div
            className="daf-overlay"
            onClick={onClose}
        >
            <section
                className="daf-modal"
                role="dialog"
                aria-modal="true"
                aria-labelledby="daf-title"
                onClick={event =>
                    event.stopPropagation()
                }
            >
                {/* Header */}
                <header className="daf-header">

                    <div className="daf-heading">

                        <div className="daf-heading-icon">
                            <FaSlidersH />
                        </div>

                        <div>
                            <h2 id="daf-title">
                                Advanced Filters
                            </h2>

                            <p>
                                Filter sample profiles
                            </p>
                        </div>

                    </div>

                    <button
                        type="button"
                        className="daf-close"
                        aria-label="Close filters"
                        onClick={onClose}
                    >
                        <FaTimes />
                    </button>

                </header>

                {/* Body */}
                <div className="daf-body">

                    <div className="daf-section">

                        <div className="daf-section-heading">

                            <h3>Interest Category</h3>

                            <button
                                type="button"
                                className="daf-reset"
                                onClick={handleReset}
                            >
                                <FaRedo />
                                Reset all
                            </button>

                        </div>

                        <select
                            aria-label="Interest category"
                            value={draft.category}
                            onChange={event =>
                                setDraft(previous => ({
                                    ...previous,
                                    category:
                                        event.target.value
                                }))
                            }
                        >
                            <option value="">
                                All categories
                            </option>

                            {categories.map(category => (
                                <option
                                    key={category}
                                    value={category}
                                >
                                    {category}
                                </option>
                            ))}
                        </select>

                    </div>

                    {/* Interests */}
                    <div className="daf-section">

                        <div className="daf-section-heading">

                            <h3>Interests</h3>

                            <span className="daf-counter">
                                {draft.interestIds.length}
                                /{MAX_INTERESTS} selected
                            </span>

                        </div>

                        <div className="daf-search">

                            <FaSearch />

                            <input
                                value={search}
                                aria-label="Search interests"
                                placeholder="Search interests..."
                                onChange={event =>
                                    setSearch(
                                        event.target.value
                                    )
                                }
                            />

                        </div>

                        {loading ? (
                            <div className="daf-loading">
                                <FaSpinner />
                                Loading interests...
                            </div>
                        ) : error ? (
                            <div className="daf-error">

                                <p>{error}</p>

                                <button
                                    type="button"
                                    onClick={() =>
                                        setReloadKey(
                                            key => key + 1
                                        )
                                    }
                                >
                                    Try again
                                </button>

                            </div>
                        ) : (
                            <div className="daf-interest-grid">

                                {visibleInterests.map(item => {
                                    const selected =
                                        draft.interestIds
                                            .includes(item.id);

                                    return (
                                        <button
                                            key={item.id}
                                            type="button"
                                            aria-pressed={selected}
                                            disabled={
                                                !selected &&
                                                draft.interestIds
                                                    .length >=
                                                MAX_INTERESTS
                                            }
                                            className={
                                                `daf-chip ${selected
                                                    ? "selected"
                                                    : ""
                                                }`
                                            }
                                            onClick={() =>
                                                toggleInterest(
                                                    item.id
                                                )
                                            }
                                        >
                                            <InterestIcon
                                                icon={item.icon}
                                            />

                                            {item.name}

                                            {selected && (
                                                <FaCheck />
                                            )}

                                        </button>
                                    );
                                })}

                                {visibleInterests.length === 0 && (
                                    <p className="daf-empty">
                                        No interests found.
                                    </p>
                                )}

                            </div>
                        )}

                    </div>

                    {/* Selected interests */}
                    {selectedInterests.length > 0 && (
                        <div className="daf-section">

                            <div className="daf-section-heading">

                                <h3>Selected Interests</h3>

                                <button
                                    type="button"
                                    className="daf-reset"
                                    onClick={() =>
                                        setDraft(previous => ({
                                            ...previous,
                                            interestIds: []
                                        }))
                                    }
                                >
                                    Clear
                                </button>

                            </div>

                            <div className="daf-interest-grid">

                                {selectedInterests.map(item => (
                                    <button
                                        type="button"
                                        key={item.id}
                                        className="daf-chip selected"
                                        onClick={() =>
                                            toggleInterest(item.id)
                                        }
                                        aria-label={
                                            `Remove ${item.name}`
                                        }
                                    >
                                        {item.name}
                                        <FaTimes />
                                    </button>
                                ))}

                            </div>

                        </div>
                    )}

                    {/* Matching mode */}
                    <div className="daf-section">

                        <h3>Matching Mode</h3>

                        <label className="daf-radio">

                            <input
                                type="radio"
                                name="matchingMode"
                                checked={
                                    !draft.matchAllInterests
                                }
                                onChange={() =>
                                    setDraft(previous => ({
                                        ...previous,
                                        matchAllInterests: false
                                    }))
                                }
                            />

                            <div>
                                <strong>Match Any</strong>

                                <span>
                                    At least one selected interest
                                </span>
                            </div>

                        </label>

                        <label className="daf-radio">

                            <input
                                type="radio"
                                name="matchingMode"
                                checked={
                                    draft.matchAllInterests
                                }
                                onChange={() =>
                                    setDraft(previous => ({
                                        ...previous,
                                        matchAllInterests: true
                                    }))
                                }
                            />

                            <div>
                                <strong>Match All</strong>

                                <span>
                                    All selected interests
                                </span>
                            </div>

                        </label>

                    </div>

                    {/* Sorting */}
                    <div className="daf-section">

                        <h3>Sort By</h3>

                        <select
                            aria-label="Sort order"
                            value={draft.sortBy}
                            onChange={event =>
                                setDraft(previous => ({
                                    ...previous,
                                    sortBy: event.target.value
                                }))
                            }
                        >
                            <option value="newest">
                                Newest first
                            </option>

                            <option value="oldest">
                                Oldest first
                            </option>

                            <option value="name_asc">
                                Name: A - Z
                            </option>

                            <option value="name_desc">
                                Name: Z - A
                            </option>
                        </select>

                    </div>

                </div>

                {/* Footer */}
                <footer className="daf-footer">

                    <button
                        type="button"
                        className="daf-btn secondary"
                        onClick={onClose}
                    >
                        Cancel
                    </button>

                    <button
                        type="button"
                        className="daf-btn primary"
                        onClick={handleApply}
                        disabled={loading || Boolean(error)}
                    >
                        <FaCheck />
                        Apply Filters
                    </button>

                </footer>

            </section>
        </div>
    );
}