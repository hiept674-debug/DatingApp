import {
    FaMusic,
    FaFilm,
    FaGamepad,
    FaBook,
    FaCamera,
    FaCode,
    FaLaptop,
    FaPalette,
    FaLeaf,
    FaBicycle,
    FaUtensils,
    FaHeadphones,
    FaGlobe,
    FaPen,
    FaSeedling,
    FaStar
} from "react-icons/fa";

const iconMap = {
    music: FaMusic,
    film: FaFilm,
    gamepad: FaGamepad,
    book: FaBook,
    camera: FaCamera,
    code: FaCode,
    laptop: FaLaptop,
    palette: FaPalette,
    leaf: FaLeaf,
    bicycle: FaBicycle,
    utensils: FaUtensils,
    headphones: FaHeadphones,
    globe: FaGlobe,
    pen: FaPen,
    seedling: FaSeedling
};

export default function InterestIcon({ icon }) {
    const IconComponent =
        iconMap[icon?.toLowerCase()] || FaStar;

    return <IconComponent aria-hidden="true" />;
}