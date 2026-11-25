export default function DiscoverSkeleton({
    count = 6
}) {
    return (
        <div
            className="discover-grid"
            aria-busy="true"
            aria-label="Loading profiles"
        >
            {Array.from({ length: count }).map(
                (_, index) => (
                    <div
                        className="discover-card"
                        key={index}
                    >
                        <div className="discover-skeleton-cover skeleton" />

                        <div className="discover-card-body">
                            <div className="skeleton skeleton-title" />

                            <div className="skeleton skeleton-text" />

                            <div className="skeleton skeleton-text short" />

                            <div className="skeleton skeleton-button" />
                        </div>
                    </div>
                )
            )}
        </div>
    );
}