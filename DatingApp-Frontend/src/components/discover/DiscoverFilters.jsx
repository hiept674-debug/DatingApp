import { FaSearch, FaTimes } from "react-icons/fa";

export default function DiscoverFilters({
    search,
    sortBy,
    onSearchChange,
    onSortChange
}) {
    return (
        <div className="discover-filters">
            <div className="discover-search">
                <FaSearch />

                <input
                    type="text"
                    placeholder="Search by name..."
                    aria-label="Search by name"
                    value={search}
                    maxLength={100}
                    onChange={(e) =>
                        onSearchChange(e.target.value)
                    }
                />

                {search && (
                    <button
                        type="button"
                        aria-label="Clear search"
                        onClick={() => onSearchChange("")}
                    >
                        <FaTimes />
                    </button>
                )}
            </div>

            <select
                aria-label="Sort profiles"
                value={sortBy}
                onChange={(e) =>
                    onSortChange(e.target.value)
                }
            >
                <option value="newest">
                    Newest first
                </option>

                <option value="oldest">
                    Oldest first
                </option>

                <option value="name_asc">
                    Name A-Z
                </option>

                <option value="name_desc">
                    Name Z-A
                </option>
            </select>
        </div>
    );
}