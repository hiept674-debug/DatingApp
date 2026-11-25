
import { Routes, Route, Navigate } from "react-router-dom";

import { AuthProvider } from "./context/AuthContext";
import ProtectedRoute from "./components/ProtectedRoute";
import MainLayout from "./layouts/MainLayout";

import Home from "./pages/Home";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Discover from "./pages/Discover";
import Profile from "./pages/Profile";
import Matches from "./pages/Matches";
import Chat from "./pages/Chat";
import ForgotPassword from "./pages/ForgotPassword";
import DiscoverFilterDemo
    from "./pages/DiscoverFilterDemo";
function App() {
    return (
        <AuthProvider>
            <Routes>
                <Route
                    path="/"
                    element={<Navigate to="/home" replace />}
                />

                {/* Public Routes */}
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />

                <Route
                    path="/forgot-password"
                    element={<ForgotPassword />}
                />

                {/* Protected Routes */}
                <Route
                    element={
                        <ProtectedRoute>
                            <MainLayout />
                        </ProtectedRoute>
                    }
                >
                    <Route path="/home" element={<Home />} />
                    <Route path="/discover" element={<Discover />} />
                    <Route path="/profile" element={<Profile />} />
                    <Route path="/matches" element={<Matches />} />
                    <Route path="/chat" element={<Chat />} />
                    <Route
                        path="/discover-filter-demo"
                        element={<DiscoverFilterDemo />}
                    />
                </Route>

                <Route
                    path="*"
                    element={<Navigate to="/home" replace />}
                />
            </Routes>
        </AuthProvider>
    );
}

export default App;
