import userService from "../services/userService";
import {
    createContext,
    useContext,
    useEffect,
    useState,
} from "react";

import authService from "../services/authService";
import api from "../services/api";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    const clearSession = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("user");

        sessionStorage.removeItem("token");
        sessionStorage.removeItem("user");

        setUser(null);
    };

    useEffect(() => {
        const savedUser =
            localStorage.getItem("user") ||
            sessionStorage.getItem("user");

        const token =
            localStorage.getItem("token") ||
            sessionStorage.getItem("token");

        if (savedUser && token) {
            try {
                setUser(JSON.parse(savedUser));
            } catch {
                clearSession();
            }
        } else {
            clearSession();
        }

        setLoading(false);

        const handleExpired = () => {
            clearSession();
        };

        window.addEventListener(
            "session-expired",
            handleExpired
        );

        return () => {
            window.removeEventListener(
                "session-expired",
                handleExpired
            );
        };
    }, []);

    const login = async (data, rememberMe = false) => {
        const result = await authService.login(data);

        if (!result.token) {
            throw new Error("API không trả về Access Token.");
        }

        const storage = rememberMe
            ? localStorage
            : sessionStorage;

        const otherStorage = rememberMe
            ? sessionStorage
            : localStorage;

        otherStorage.removeItem("token");
        otherStorage.removeItem("user");

        const userData = {
            id: result.id,
            email: result.email,
            phone: result.phone,
            fullName: result.fullName,
        };

        storage.setItem("token", result.token);
        storage.setItem("user", JSON.stringify(userData));

        setUser(userData);
        try {
            const profile = await userService.getMyProfile();

            const updatedUser = {
                ...userData,
                avatarUrl: profile.avatarUrl ?? null,
            };

            storage.setItem(
                "user",
                JSON.stringify(updatedUser)
            );

            setUser(updatedUser);
        } catch (error) {
            console.warn("Unable to load account avatar.");
        }

        return result;
    };



    const updateUser = (newData) => {
        if (!user) return;

        const storage = localStorage.getItem("token")
            ? localStorage
            : sessionStorage;

        if (!storage.getItem("token")) return;

        const updatedUser = {
            ...user,
            ...newData,
        };

        storage.setItem(
            "user",
            JSON.stringify(updatedUser)
        );

        setUser(updatedUser);
    };



    const logout = async () => {
        try {
            await api.post("/auth/logout");
        } catch {
            // Dù API logout lỗi, vẫn xóa phiên trên trình duyệt.
        } finally {
            clearSession();
        }
    };

    return (
        <AuthContext.Provider
            value={{
                user,
                loading,
                isAuthenticated: !!user,
                login,
                logout,
                updateUser,
            }}
        >
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    return useContext(AuthContext);
}
