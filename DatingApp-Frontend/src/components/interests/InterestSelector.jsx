import { useEffect, useMemo, useState } from "react";

import {
    FaEdit,
    FaCheck,
    FaTimes,
    FaSpinner,
    FaStar,
    FaExclamationCircle,
    FaRedo,
    FaSearch
} from "react-icons/fa";

import interestService from "../../services/interestService";
import InterestIcon from "./InterestIcon";

import "./InterestSelector.css";

const MAX_INTERESTS = 10;

export default function InterestSelector() {
    const [interests, setInterests] = useState([]);
    const [savedIds, setSavedIds] = useState([]);
    const [selectedIds, setSelectedIds] = useState([]);

    const [editing, setEditing] = useState(false);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);

    const [search, setSearch] = useState("");
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    const [reloadKey, setReloadKey] = useState(0);

    // ====================================
    // LOAD DATA
    // ====================================

    useEffect(() => {
        const controller = new AbortController();

        const loadInterests = async () => {
            setLoading(true);
            setError("");

            try {
                const [allData, myData] =
                    await Promise.all([
                        interestService.getAll(
                            controller.signal
                        ),

                        interestService.getMyInterests(
                            controller.signal
                        )
                    ]);

                if (controller.signal.aborted) return;

                const ids = myData.map(item => item.id);

                setInterests(allData);
                setSavedIds(ids);
                setSelectedIds(ids);
            } catch (err) {
                if (controller.signal.aborted) return;

                setError(getErrorMessage(err));
            } finally {
                if (!controller.signal.aborted) {
                    setLoading(false);
                }
            }
        };

        loadInterests();

        return () => controller.abort();

    }, [reloadKey]);

    // ====================================
    // SEARCH & GROUP BY CATEGORY
    // ====================================

    const groupedInterests = useMemo(() => {
        const keyword = search.trim().toLowerCase();

        const filtered = interests.filter(item =>
            item.name.toLowerCase().includes(keyword) ||
            item.category.toLowerCase().includes(keyword)
        );

        return filtered.reduce((groups, item) => {
            const category =
                item.category || "Other";

            if (!groups.has(category)) {
                groups.set(category, []);
            }

            groups.get(category).push(item);

            return groups;
        }, new Map());

    }, [interests, search]);

    const savedInterests = useMemo(() => {
        return interests.filter(item =>
            savedIds.includes(item.id)
        );
    }, [interests, savedIds]);

    const hasChanges =
        savedIds.length !== selectedIds.length ||
        selectedIds.some(id => !savedIds.includes(id));

    // ====================================
    // ERROR HANDLING
    // ====================================

    function getErrorMessage(err) {
        const data = err.response?.data;

        if (typeof data === "string") {
            return data;
        }

        if (data?.errors) {
            return Object.values(data.errors)
                .flat()
                .join(" ");
        }

        return data?.message ||
            data?.title ||
            "Unable to process interests. Please try again.";
    }

    // ====================================
    // EDIT
    // ====================================

    const handleEdit = () => {
        setSelectedIds([...savedIds]);
        setEditing(true);
        setSearch("");
        setError("");
        setSuccess("");
    };

    // ====================================
    // SELECT / UNSELECT
    // ====================================

    const toggleInterest = (id) => {
        if (saving) return;

        setError("");
        setSuccess("");

        if (selectedIds.includes(id)) {
            setSelectedIds(prev =>
                prev.filter(item => item !== id)
            );

            return;
        }

        if (selectedIds.length >= MAX_INTERESTS) {
            setError(
                "You can select up to 10 interests."
            );

            return;
        }

        setSelectedIds(prev => [...prev, id]);
    };

    // ====================================
    // CANCEL
    // ====================================

    const handleCancel = () => {
        setSelectedIds([...savedIds]);
        setEditing(false);
        setSearch("");
        setError("");
    };

    // ====================================
    // SAVE
    // ====================================

    const handleSave = async () => {
        if (saving || !hasChanges) return;

        setSaving(true);
        setError("");
        setSuccess("");

        try {
            const result =
                await interestService.updateMyInterests(
                    selectedIds
                );

            setSavedIds([...selectedIds]);
            setEditing(false);
            setSearch("");

            setSuccess(
                result.message ||
                "Interests updated successfully."
            );

        } catch (err) {
            setError(getErrorMessage(err));
        } finally {
            setSaving(false);
        }
    };

    // ====================================
    // RENDER
    // ====================================

    return (
        <section className="interest-panel">

            <div className="interest-header">

                <div className="interest-heading">

                    <div className="interest-heading-icon">
                        <FaStar />
                    </div>

                    <div>
                        <h3>My Interests</h3>

                        <p>Things I enjoy doing</p>
                    </div>

                </div>

                {!editing && !loading && (
                    <button
                        type="button"
                        className="interest-btn interest-btn-outline"
                        onClick={handleEdit}
                        disabled={interests.length === 0}
                    >
                        <FaEdit />
                        Edit
                    </button>
                )}

            </div>

            {/* Success Message */}

            {success && (
                <div
                    className="interest-alert success"
                    role="status"
                >
                    <FaCheck />
                    {success}
                </div>
            )}

            {/* Error Message */}

            {error && (
                <div
                    className="interest-alert error"
                    role="alert"
                >
                    <FaExclamationCircle />
                    {error}
                </div>
            )}

            {/* Loading State */}

            {loading ? (
                <div className="interest-state">

                    <FaSpinner className="interest-spinner" />

                    <p>Loading interests...</p>

                </div>

            ) : error && interests.length === 0 ? (

                <div className="interest-state">

                    <p>Unable to load interests.</p>

                    <button
                        className="interest-btn interest-btn-outline"
                        onClick={() =>
                            setReloadKey(prev => prev + 1)
                        }
                    >
                        <FaRedo />
                        Try again
                    </button>

                </div>

            ) : !editing ? (

                /* VIEW MODE */

                <div className="interest-view">

                    {savedInterests.length > 0 ? (

                        <div className="interest-chips">

                            {savedInterests.map(item => (
                                <span
                                    key={item.id}
                                    className="interest-chip selected"
                                >
                                    <InterestIcon icon={item.icon} />

                                    {item.name}
                                </span>
                            ))}

                        </div>

                    ) : (

                        <div className="interest-empty">

                            <FaStar />

                            <h4>No interests selected</h4>

                            <p>
                                {interests.length === 0
                                    ? "No interests are available yet."
                                    : "Add activities and topics you enjoy."}
                            </p>

                            {interests.length > 0 && (
                                <button
                                    className="interest-btn interest-btn-primary"
                                    onClick={handleEdit}
                                >
                                    Add Interests
                                </button>
                            )}

                        </div>
                    )}

                    <p className="interest-count">
                        {savedIds.length} interests selected
                    </p>

                </div>

            ) : (

                /* EDIT MODE */

                <div className="interest-editor">

                    <div className="interest-counter">

                        <span>Select your interests</span>

                        <strong>
                            {selectedIds.length}/{MAX_INTERESTS}
                        </strong>

                    </div>

                    <div
                        className="interest-progress"
                        role="progressbar"
                        aria-valuenow={selectedIds.length}
                        aria-valuemin={0}
                        aria-valuemax={MAX_INTERESTS}
                        aria-label="Selected interests"
                    >
                        <div
                            style={{
                                width:
                                    `${selectedIds.length / MAX_INTERESTS * 100}%`
                            }}
                        />
                    </div>

                    {/* Search */}

                    <div className="interest-search">

                        <FaSearch />

                        <input
                            type="search"
                            placeholder="Search interests..."
                            aria-label="Search interests"
                            value={search}
                            onChange={event =>
                                setSearch(event.target.value)
                            }
                        />

                    </div>

                    {/* Categories */}

                    <div className="interest-categories">

                        {Array.from(groupedInterests.entries())
                            .map(([category, items]) => (

                                <div
                                    key={category}
                                    className="interest-category"
                                >

                                    <h4>{category}</h4>

                                    <div className="interest-chips">

                                        {items.map(item => {
                                            const isSelected =
                                                selectedIds.includes(item.id);

                                            return (
                                                <button
                                                    type="button"
                                                    key={item.id}
                                                    aria-pressed={isSelected}
                                                    disabled={saving}
                                                    className={
                                                        `interest-chip ${isSelected
                                                            ? "selected"
                                                            : ""
                                                        }`
                                                    }
                                                    onClick={() =>
                                                        toggleInterest(item.id)
                                                    }
                                                >
                                                    <InterestIcon
                                                        icon={item.icon}
                                                    />

                                                    {item.name}

                                                    {isSelected && <FaCheck />}

                                                </button>
                                            );
                                        })}

                                    </div>

                                </div>
                            ))}

                        {groupedInterests.size === 0 && (
                            <p className="interest-no-results">
                                No matching interests found.
                            </p>
                        )}

                    </div>

                    {/* Footer */}

                    <div className="interest-footer">

                        <button
                            type="button"
                            className="interest-clear"
                            disabled={
                                saving ||
                                selectedIds.length === 0
                            }
                            onClick={() => {
                                setSelectedIds([]);
                                setError("");
                            }}
                        >
                            Clear all
                        </button>

                        <div className="interest-actions">

                            <button
                                type="button"
                                className="interest-btn interest-btn-outline"
                                disabled={saving}
                                onClick={handleCancel}
                            >
                                <FaTimes />
                                Cancel
                            </button>

                            <button
                                type="button"
                                className="interest-btn interest-btn-primary"
                                disabled={saving || !hasChanges}
                                onClick={handleSave}
                            >
                                {saving ? (
                                    <FaSpinner
                                        className="interest-spinner"
                                    />
                                ) : (
                                    <FaCheck />
                                )}

                                {saving
                                    ? "Saving..."
                                    : "Save Interests"}

                            </button>

                        </div>

                    </div>

                </div>
            )}

        </section>
    );
}