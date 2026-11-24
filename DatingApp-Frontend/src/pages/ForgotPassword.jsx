
import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { passwordResetApi } from "../services/passwordResetApi";
import "./ForgotPassword.css";
export default function ForgotPassword() {
    const navigate = useNavigate();

    const [step, setStep] = useState(1);
    const [email, setEmail] = useState("");
    const [otp, setOtp] = useState("");
    const [resetToken, setResetToken] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [message, setMessage] = useState("");

    async function handleSubmit(event) {
        event.preventDefault();
        setError("");
        setMessage("");
        setLoading(true);

        try {
            if (step === 1) {
                await passwordResetApi.forgotPassword(email.trim());

                setMessage(
                    "Nếu email đã được đăng ký, mã OTP sẽ được gửi đến hộp thư."
                );
                setStep(2);
            }

            if (step === 2) {
                const result = await passwordResetApi.verifyResetCode(
                    email.trim(),
                    otp.trim()
                );

                if (!result.resetToken) {
                    throw new Error("Backend không trả về resetToken.");
                }

                // Giữ token trong bộ nhớ, không lưu vào localStorage.
                setResetToken(result.resetToken);
                setMessage("Xác minh OTP thành công.");
                setStep(3);
            }

            if (step === 3) {
                if (newPassword.length < 8) {
                    throw new Error("Mật khẩu phải có ít nhất 8 ký tự.");
                }

                if (newPassword !== confirmPassword) {
                    throw new Error("Xác nhận mật khẩu không khớp.");
                }

                await passwordResetApi.resetPassword(
                    email.trim(),
                    resetToken,
                    newPassword,
                    confirmPassword
                );

                setResetToken("");
                setOtp("");
                setNewPassword("");
                setConfirmPassword("");

                navigate("/login", {
                    replace: true,
                    state: {
                        message: "Đổi mật khẩu thành công. Hãy đăng nhập lại.",
                    },
                });
            }
        } catch (err) {
            setError(err.message || "Có lỗi xảy ra.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="forgot-page">
            <form className="forgot-card" onSubmit={handleSubmit}>
                <h1>Quên mật khẩu</h1>

                <p className="forgot-subtitle">
                    {step === 1 && "Nhập email để nhận mã xác minh."}
                    {step === 2 && "Nhập mã OTP đã gửi đến email của bạn."}
                    {step === 3 && "Tạo mật khẩu mới cho tài khoản."}
                </p>

                <div className="forgot-progress">
                    <span className={step >= 1 ? "active" : ""}>1</span>
                    <span className={step >= 2 ? "active" : ""}>2</span>
                    <span className={step >= 3 ? "active" : ""}>3</span>
                </div>

                {error && <div className="forgot-error">{error}</div>}
                {message && <div className="forgot-success">{message}</div>}

                {step === 1 && (
                    <label>
                        Email
                        <input
                            type="email"
                            placeholder="example@gmail.com"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </label>
                )}

                {step === 2 && (
                    <>
                        <p className="forgot-email">{email}</p>

                        <label>
                            Mã OTP
                            <input
                                type="text"
                                inputMode="numeric"
                                maxLength={6}
                                pattern="[0-9]{6}"
                                placeholder="Nhập 6 chữ số"
                                value={otp}
                                onChange={(e) =>
                                    setOtp(e.target.value.replace(/\D/g, ""))
                                }
                                required
                            />
                        </label>

                        <button
                            type="button"
                            className="forgot-link-button"
                            disabled={loading}
                            onClick={async () => {
                                setError("");
                                setMessage("");

                                try {
                                    setLoading(true);
                                    await passwordResetApi.forgotPassword(email.trim());
                                    setMessage(
                                        "Nếu email đã được đăng ký, mã OTP sẽ được gửi."
                                    );
                                } catch (err) {
                                    setError(err.message);
                                } finally {
                                    setLoading(false);
                                }
                            }}
                        >
                            Gửi lại OTP
                        </button>
                    </>
                )}

                {step === 3 && (
                    <>
                        <label>
                            Mật khẩu mới
                            <input
                                type="password"
                                value={newPassword}
                                onChange={(e) => setNewPassword(e.target.value)}
                                minLength={8}
                                autoComplete="new-password"
                                required
                            />
                        </label>

                        <label>
                            Xác nhận mật khẩu
                            <input
                                type="password"
                                value={confirmPassword}
                                onChange={(e) => setConfirmPassword(e.target.value)}
                                minLength={8}
                                autoComplete="new-password"
                                required
                            />
                        </label>
                    </>
                )}

                <button type="submit" disabled={loading}>
                    {loading
                        ? "Đang xử lý..."
                        : step === 1
                            ? "Gửi mã OTP"
                            : step === 2
                                ? "Xác minh OTP"
                                : "Đổi mật khẩu"}
                </button>

                <Link className="forgot-back" to="/login">
                    Quay lại đăng nhập
                </Link>
            </form>
        </div>
    );
}
