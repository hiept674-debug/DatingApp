import {
    FaChevronLeft,
    FaChevronRight
} from "react-icons/fa";

export default function Pagination({
    pageNumber,
    totalPages,
    onPageChange
}) {
    if (totalPages <= 1) {
        return null;
    }

    const start = Math.max(
        1,
        Math.min(pageNumber - 2, totalPages - 4)
    );

    const end = Math.min(
        totalPages,
        start + 4
    );

    const pages = Array.from(
        { length: end - start + 1 },
        (_, index) => start + index
    );

    return (
        <nav
            className="discover-pagination"
            aria-label="Pagination"
        >
            <button
                aria-label="Previous page"
                disabled={pageNumber <= 1}
                onClick={() =>
                    onPageChange(pageNumber - 1)
                }
            >
                <FaChevronLeft />
            </button>

            {pages.map((page) => (
                <button
                    key={page}
                    aria-current={
                        page === pageNumber
                            ? "page"
                            : undefined
                    }
                    className={
                        page === pageNumber
                            ? "active"
                            : ""
                    }
                    onClick={() =>
                        onPageChange(page)
                    }
                >
                    {page}
                </button>
            ))}

            <button
                aria-label="Next page"
                disabled={pageNumber >= totalPages}
                onClick={() =>
                    onPageChange(pageNumber + 1)
                }
            >
                <FaChevronRight />
            </button>
        </nav>
    );
}