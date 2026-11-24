const API_URL = import.meta.env.VITE_API_URL.replace(/\/+$/, "");

async function postJson(endpoint, data) {
    const response = await fetch(
        `${API_URL}/auth/${endpoint}`,
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(data),
        }
    );

    const responseText = await response.text();

    let result = {};

    if (responseText.trim()) {
        try {
            result = JSON.parse(responseText);
        } catch {
            throw new Error(
                `Backend trả về dữ liệu không hợp lệ (HTTP ${response.status}).`
            );
        }
    }

    if (!response.ok) {
        throw new Error(
            result.message ||
            result.title ||
            `Yêu cầu thất bại (HTTP ${response.status}).`
        );
    }

    return result;
}

export const passwordResetApi = {
    forgotPassword: (email) =>
        postJson("forgot-password", { email }),

    verifyResetCode: (email, code) =>
        postJson("verify-reset-code", { email, code }),

    resetPassword: (
        email,
        resetToken,
        newPassword,
        confirmPassword
    ) =>
        postJson("reset-password", {
            email,
            resetToken,
            newPassword,
            confirmPassword,
        }),
};