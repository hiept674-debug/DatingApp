import LogoutButton from "./LogoutButton";

function Navbar() {
    return (
        <nav className="navbar navbar-expand-lg navbar-light bg-white shadow-sm">
            <div className="container">
                <a className="navbar-brand" href="/">
                    DatingApp ❤️
                </a>

                <LogoutButton />
            </div>
        </nav>
    );
}

export default Navbar;