
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace DatingAppAPI.Services;

public class EmailService {
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IConfiguration configuration,
        ILogger<EmailService> logger)
{
    _configuration = configuration;
    _logger = logger;
}

    public async Task SendOtpAsync(
    string toEmail,
    string otp)
{
    var sender = _configuration[
        "EmailSettings:SenderEmail"];

    var password = _configuration[
        "EmailSettings:AppPassword"];

    if (string.IsNullOrWhiteSpace(sender) ||
        string.IsNullOrWhiteSpace(password)) {
        throw new InvalidOperationException(
            "Chưa cấu hình Gmail SMTP.");
    }

    var message = new MimeMessage();

    message.From.Add(
        MailboxAddress.Parse(sender));

    message.To.Add(
        MailboxAddress.Parse(toEmail));

    message.Subject =
        "DatingApp - Mã OTP đặt lại mật khẩu";

    message.Body = new TextPart("plain")
    {
        Text =
            "Xin chào,\n\n" +
            $"Mã OTP của bạn là: {otp}\n\n" +
                "Mã có hiệu lực trong 5 phút.\n" +
                "Nếu bạn không yêu cầu đổi mật khẩu, " +
                "hãy bỏ qua email này.\n\n" +
                "DatingApp"
    };

        using var client = new SmtpClient();

    try {
        _logger.LogInformation(
            "Đang kết nối Gmail SMTP.");

        await client.ConnectAsync(
            "smtp.gmail.com",
            587,
            SecureSocketOptions.StartTls);

        await client.AuthenticateAsync(
            sender,
            password);

        await client.SendAsync(message);

        _logger.LogInformation(
            "SMTP đã chấp nhận email OTP gửi đến {Email}.",
            toEmail);
    }
    finally {
        if (client.IsConnected) {
            await client.DisconnectAsync(true);
        }
    }
}
}
