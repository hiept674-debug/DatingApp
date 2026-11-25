import { useState } from "react";
import DiscoverAdvancedFilters
    from "../components/discover/DiscoverAdvancedFilters";
import filterSampleProfiles
    from "../utils/filterSampleProfiles";

const SAMPLE_PROFILES = [
    {
        id: 1,
        fullName: "Sample 01",
        interests: [{ id: 10, isActive: true }]
    },
    {
        id: 2,
        fullName: "Sample 02",
        interests: [{ id: 11, isActive: true }]
    },
    {
        id: 3,
        fullName: "Sample 03",
        interests: [
            { id: 10, isActive: true },
            { id: 11, isActive: true }
        ]
    },
    {
        id: 4,
        fullName: "Sample 04",
        interests: []
    }
];

export default function DiscoverFilterDemo() {
    const [open, setOpen] = useState(false);

    const [filters, setFilters] = useState({
        category: "",
        interestIds: [],
        matchAllInterests: false,
        sortBy: "newest"
    });

    const results = filterSampleProfiles(
        SAMPLE_PROFILES,
        filters
    );

    return (
        <div style={{ padding: 24 }}>
            <h1>Discover Filter Test</h1>

            <button onClick={() => setOpen(true)}>
                Filters
            </button>

            <h3>{results.length} profiles found</h3>

            {results.map(profile => (
                <div key={profile.id}>
                    {profile.fullName}
                </div>
            ))}

            {open && (
                <DiscoverAdvancedFilters
                    filters={filters}
                    onClose={() => setOpen(false)}
                    onApply={(newFilters) => {
                        setFilters(newFilters);
                        setOpen(false);
                    }}
                />
            )}
        </div>
    );
}