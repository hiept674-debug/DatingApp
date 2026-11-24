
import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { FaHeart, FaEye, FaEyeSlash } from "react-icons/fa";
import { MdEmail, MdLock } from "react-icons/md";
import "./Auth.css";

function Login() {
    const navigate = useNavigate();
    const { login } = useAuth();

    const [form, setForm] = useState({
        email: "",
        password: "",
    });

    const [showPassword, setShowPassword] = useState(false);
    const [rememberMe, setRememberMe] = useState(false);
    const [errors, setErrors] = useState({});
    const [apiError, setApiError] = useState("");
    const [loading, setLoading] = useState(false);

    const handleChange = (e) => {
        setForm({
            ...form,
            [e.target.name]: e.target.value,
        });

        setErrors({});
        setApiError("");
    };

    const validate = () => {
        const newErrors = {};

        if (!form.email.trim()) {
            newErrors.email = "Vui lòng nhập email.";
        } else if (
            !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)
        ) {
            newErrors.email = "Email không hợp lệ.";
        }

        if (!form.password) {
            newErrors.password = "Vui lòng nhập mật khẩu.";
        }

        setErrors(newErrors);

        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setApiError("");

        if (!validate()) return;

        try {
            setLoading(true);

            await login(form, rememberMe);

            navigate("/home", { replace: true });
        } catch (error) {
            setApiError(
                error.response?.data?.message ||
                error.response?.data?.title ||
                error.message ||
                "Đăng nhập thất bại. Vui lòng thử lại."
            );
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="auth-page">
            <div className="auth-container">

                <div className="auth-banner">
                    <div className="banner-content">
                        <div className="brand-logo">
                            <FaHeart />
                            DatingApp
                        </div>

                        <h1>Find Your Perfect Match</h1>

                        <p>
                            Meet new people, make friends and discover
                            meaningful connections.
                        </p>

                        <div className="banner-decoration">
                            <FaHeart />
                        </div>

                        <span>Love starts with a connection.</span>
                    </div>
                </div>

                <div className="auth-form-section">
                    <div className="auth-form">

                        <div className="mobile-logo">
                            <FaHeart /> DatingApp
                        </div>

                        <h2>Welcome Back!</h2>

                        <p className="auth-subtitle">
                            Sign in to continue your journey.
                        </p>

                        {apiError && (
                            <div className="alert alert-danger">
                                {apiError}
                            </div>
                        )}

                        <form onSubmit={handleSubmit} noValidate>

                            <div className="form-group">
                                <label>Email Address</label>

                                <div className="input-wrapper">
                                    <MdEmail />

                                    <input
                                        type="email"
                                        name="email"
                                        placeholder="Enter your email"
                                        value={form.email}
                                        onChange={handleChange}
                                        className={errors.email ? "input-error" : ""}
                                    />
                                </div>

                                {errors.email && (
                                    <small className="error-text">
                                        {errors.email}
                                    </small>
                                )}
                            </div>

                            <div className="form-group">
                                <label>Password</label>

                                <div className="input-wrapper">
                                    <MdLock />

                                    <input
                                        type={showPassword ? "text" : "password"}
                                        name="password"
                                        placeholder="Enter your password"
                                        value={form.password}
                                        onChange={handleChange}
                                        className={errors.password ? "input-error" : ""}
                                    />

                                    <button
                                        type="button"
                                        className="password-toggle"
                                        onClick={() => setShowPassword(!showPassword)}
                                        aria-label="Toggle password visibility"
                                    >
                                        {showPassword ? <FaEyeSlash /> : <FaEye />}
                                    </button>
                                </div>

                                {errors.password && (
                                    <small className="error-text">
                                        {errors.password}
                                    </small>
                                )}
                            </div>

                            <div className="auth-options">
                                <label className="remember-label">
                                    <input
                                        type="checkbox"
                                        checked={rememberMe}
                                        onChange={(e) =>
                                            setRememberMe(e.target.checked)
                                        }
                                    />
                                    Remember me
                                </label>

                                <Link to="/forgot-password">
                                    Forgot password?
                                </Link>
                            </div>

                            <button
                                type="submit"
                                className="auth-button"
                                disabled={loading}
                            >
                                {loading ? "Signing in..." : "Sign In"}
                            </button>

                        </form>

                        <div className="auth-switch">
                            Don't have an account?
                            <Link to="/register"> Create Account</Link>
                        </div>

                        <div className="auth-footer">
                            © 2026 DatingApp. All rights reserved.
                        </div>

                    </div>
                </div>

            </div>
        </div>
    );
}

export default Login;
