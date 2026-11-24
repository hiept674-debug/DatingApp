
import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { FaHeart, FaEye, FaEyeSlash } from "react-icons/fa";
import { MdEmail, MdLock, MdPerson, MdPhone } from "react-icons/md";
import authService from "../services/authService";
import "./Auth.css";

function Register() {
    const navigate = useNavigate();

    const [form, setForm] = useState({
        fullName: "",
        email: "",
        phone: "",
        password: "",
        confirmPassword: "",
        dateOfBirth: "",
        gender: "",
        city: "",
        bio: "",
    });

    const [showPassword, setShowPassword] = useState(false);
    const [showConfirm, setShowConfirm] = useState(false);
    const [agree, setAgree] = useState(false);
    const [errors, setErrors] = useState({});
    const [apiError, setApiError] = useState("");
    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState("");

    const handleChange = (e) => {
        const { name, value } = e.target;

        setForm((prev) => ({
            ...prev,
            [name]: value,
        }));

        setErrors((prev) => ({
            ...prev,
            [name]: "",
        }));

        setApiError("");
    };

    const passwordStrength = () => {
        let score = 0;

        if (form.password.length >= 8) score++;
        if (/[A-Z]/.test(form.password)) score++;
        if (/[a-z]/.test(form.password)) score++;
        if (/[0-9]/.test(form.password)) score++;
        if (/[^A-Za-z0-9]/.test(form.password)) score++;

        return score;
    };

    const validate = () => {
        const newErrors = {};

        if (!form.fullName.trim()) {
            newErrors.fullName = "Vui lòng nhập họ tên.";
        } else if (form.fullName.trim().length < 2) {
            newErrors.fullName = "Họ tên phải có ít nhất 2 ký tự.";
        }

        if (!form.email.trim()) {
            newErrors.email = "Vui lòng nhập email.";
        } else if (
            !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)
        ) {
            newErrors.email = "Email không hợp lệ.";
        }

        if (!/^0\d{9}$/.test(form.phone)) {
            newErrors.phone = "Số điện thoại phải có 10 số và bắt đầu bằng 0.";
        }

        if (passwordStrength() < 4) {
            newErrors.password =
                "Mật khẩu cần ít nhất 8 ký tự, chữ hoa, chữ thường và số.";
        }

        if (form.password !== form.confirmPassword) {
            newErrors.confirmPassword = "Mật khẩu xác nhận không khớp.";
        }

        if (!form.dateOfBirth) {
            newErrors.dateOfBirth = "Vui lòng chọn ngày sinh.";
        } else {
            const birthday = new Date(form.dateOfBirth);
            const today = new Date();
            let age = today.getFullYear() - birthday.getFullYear();

            const monthDiff = today.getMonth() - birthday.getMonth();

            if (
                monthDiff < 0 ||
                (monthDiff === 0 && today.getDate() < birthday.getDate())
            ) {
                age--;
            }

            if (birthday > today || age < 18) {
                newErrors.dateOfBirth =
                    "Bạn phải đủ 18 tuổi để đăng ký.";
            }
        }

        if (!form.gender) {
            newErrors.gender = "Vui lòng chọn giới tính.";
        }

        if (!form.city.trim()) {
            newErrors.city = "Vui lòng nhập thành phố.";
        }

        if (!agree) {
            newErrors.agree = "Bạn cần đồng ý với điều khoản sử dụng.";
        }

        setErrors(newErrors);

        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        setApiError("");
        setSuccess("");

        if (!validate()) return;

        try {
            setLoading(true);

            const {
                confirmPassword,
                ...registerData
            } = form;

            await authService.register(registerData);

            setSuccess("Đăng ký thành công! Hãy đăng nhập.");

            setTimeout(() => {
                navigate("/login", {
                    state: { email: form.email },
                });
            }, 1200);

        } catch (error) {
            setApiError(
                error.response?.data?.message ||
                error.response?.data?.title ||
                error.message ||
                "Đăng ký thất bại."
            );
        } finally {
            setLoading(false);
        }
    };

    const strength = passwordStrength();

    return (
        <div className="auth-page">
            <div className="auth-container register-container">

                <div className="auth-banner">
                    <div className="banner-content">
                        <div className="brand-logo">
                            <FaHeart /> DatingApp
                        </div>

                        <h1>Start Your Love Story</h1>

                        <p>
                            Create your account and discover
                            wonderful connections.
                        </p>

                        <div className="banner-decoration">
                            <FaHeart />
                        </div>

                        <span>Your next connection starts here.</span>
                    </div>
                </div>

                <div className="auth-form-section">
                    <div className="auth-form register-form">

                        <div className="mobile-logo">
                            <FaHeart /> DatingApp
                        </div>

                        <h2>Create Account</h2>

                        <p className="auth-subtitle">
                            Join DatingApp and meet new people.
                        </p>

                        {apiError && (
                            <div className="alert alert-danger">
                                {apiError}
                            </div>
                        )}

                        {success && (
                            <div className="alert alert-success">
                                {success}
                            </div>
                        )}

                        <form onSubmit={handleSubmit} noValidate>

                            <div className="form-group">
                                <label>Full Name *</label>

                                <div className="input-wrapper">
                                    <MdPerson />

                                    <input
                                        name="fullName"
                                        placeholder="Enter your full name"
                                        value={form.fullName}
                                        onChange={handleChange}
                                    />
                                </div>

                                {errors.fullName && (
                                    <small className="error-text">
                                        {errors.fullName}
                                    </small>
                                )}
                            </div>

                            <div className="form-group">
                                <label>Email Address *</label>

                                <div className="input-wrapper">
                                    <MdEmail />

                                    <input
                                        type="email"
                                        name="email"
                                        placeholder="example@gmail.com"
                                        value={form.email}
                                        onChange={handleChange}
                                    />
                                </div>

                                {errors.email && (
                                    <small className="error-text">
                                        {errors.email}
                                    </small>
                                )}
                            </div>

                            <div className="form-group">
                                <label>Phone Number *</label>

                                <div className="input-wrapper">
                                    <MdPhone />

                                    <input
                                        type="tel"
                                        name="phone"
                                        placeholder="0123456789"
                                        maxLength={10}
                                        value={form.phone}
                                        onChange={handleChange}
                                    />
                                </div>

                                {errors.phone && (
                                    <small className="error-text">
                                        {errors.phone}
                                    </small>
                                )}
                            </div>

                            <div className="form-group">
                                <label>Password *</label>

                                <div className="input-wrapper">
                                    <MdLock />

                                    <input
                                        type={showPassword ? "text" : "password"}
                                        name="password"
                                        placeholder="Create a password"
                                        value={form.password}
                                        onChange={handleChange}
                                    />

                                    <button
                                        type="button"
                                        className="password-toggle"
                                        onClick={() =>
                                            setShowPassword(!showPassword)
                                        }
                                    >
                                        {showPassword ? <FaEyeSlash /> : <FaEye />}
                                    </button>
                                </div>

                                <div className="strength-track">
                                    <div
                                        className="strength-fill"
                                        style={{
                                            width: `${(strength / 5) * 100}%`,
                                        }}
                                    />
                                </div>

                                <small className="strength-label">
                                    Password strength: {
                                        ["Very weak", "Weak", "Fair", "Good", "Strong", "Very strong"][strength]
                                    }
                                </small>

                                {errors.password && (
                                    <small className="error-text">
                                        {errors.password}
                                    </small>
                                )}
                            </div>

                            <div className="form-group">
                                <label>Confirm Password *</label>

                                <div className="input-wrapper">
                                    <MdLock />

                                    <input
                                        type={showConfirm ? "text" : "password"}
                                        name="confirmPassword"
                                        placeholder="Confirm your password"
                                        value={form.confirmPassword}
                                        onChange={handleChange}
                                    />

                                    <button
                                        type="button"
                                        className="password-toggle"
                                        onClick={() =>
                                            setShowConfirm(!showConfirm)
                                        }
                                    >
                                        {showConfirm ? <FaEyeSlash /> : <FaEye />}
                                    </button>
                                </div>

                                {errors.confirmPassword && (
                                    <small className="error-text">
                                        {errors.confirmPassword}
                                    </small>
                                )}
                            </div>

                            <div className="row">
                                <div className="col-md-6 form-group">
                                    <label>Date of Birth *</label>

                                    <input
                                        type="date"
                                        name="dateOfBirth"
                                        className="form-control custom-input"
                                        value={form.dateOfBirth}
                                        onChange={handleChange}
                                    />

                                    {errors.dateOfBirth && (
                                        <small className="error-text">
                                            {errors.dateOfBirth}
                                        </small>
                                    )}
                                </div>

                                <div className="col-md-6 form-group">
                                    <label>Gender *</label>

                                    <select
                                        name="gender"
                                        className="form-select custom-input"
                                        value={form.gender}
                                        onChange={handleChange}
                                    >
                                        <option value="">Select gender</option>
                                        <option value="Male">Male</option>
                                        <option value="Female">Female</option>
                                        <option value="Other">Other</option>
                                    </select>

                                    {errors.gender && (
                                        <small className="error-text">
                                            {errors.gender}
                                        </small>
                                    )}
                                </div>
                            </div>

                            <div className="form-group">
                                <label>City *</label>

                                <input
                                    name="city"
                                    className="form-control custom-input"
                                    placeholder="Your city"
                                    value={form.city}
                                    onChange={handleChange}
                                />

                                {errors.city && (
                                    <small className="error-text">
                                        {errors.city}
                                    </small>
                                )}
                            </div>

                            <div className="form-group">
                                <label>About Me</label>

                                <textarea
                                    name="bio"
                                    className="form-control custom-input"
                                    rows="3"
                                    maxLength={500}
                                    placeholder="Tell us something about yourself..."
                                    value={form.bio}
                                    onChange={handleChange}
                                />

                                <small className="text-muted">
                                    {form.bio.length}/500 characters
                                </small>
                            </div>

                            <div className="terms-group">
                                <input
                                    type="checkbox"
                                    checked={agree}
                                    onChange={(e) => setAgree(e.target.checked)}
                                />

                                <span>
                                    I agree to the Terms of Service and Privacy Policy.
                                </span>
                            </div>

                            {errors.agree && (
                                <small className="error-text">
                                    {errors.agree}
                                </small>
                            )}

                            <button
                                type="submit"
                                className="auth-button"
                                disabled={loading}
                            >
                                {loading ? "Creating account..." : "Create Account"}
                            </button>

                        </form>

                        <div className="auth-switch">
                            Already have an account?
                            <Link to="/login"> Sign In</Link>
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

export default Register;
