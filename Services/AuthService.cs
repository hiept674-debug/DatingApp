using DatingAppAPI.Data;
using DatingAppAPI.DTOs.Auth;
using DatingAppAPI.Models;
using DatingAppAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
namespace DatingAppAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public AuthService(
            AppDbContext context,
            IAuthRepository authRepository,
            IConfiguration configuration,
            EmailService emailService)
        {
            _context = context;
            _authRepository = authRepository;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                throw new InvalidOperationException("Email đã tồn tại.");
            }

            if (await _context.Users.AnyAsync(u => u.Phone == dto.Phone))
            {
                throw new InvalidOperationException("Số điện thoại đã tồn tại.");
            }

            var user = new User
            {
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Bio = dto.Bio ?? string.Empty,
                City = dto.City,
                AvatarUrl = dto.AvatarUrl,
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone,
                FullName = user.FullName,
                Token = GenerateJwtToken(user)
            };
        }


        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _authRepository.GetByEmailAsync(dto.Email);

            if (user == null || !user.IsActive)
            {
                return null;
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(
                dto.Password,
                user.PasswordHash);

            if (!passwordValid)
            {
                return null;
            }

            return new AuthResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone,
                FullName = user.FullName,
                Token = GenerateJwtToken(user)
            };
        }


        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKeyText = jwtSettings["SecretKey"];

            if (string.IsNullOrWhiteSpace(secretKeyText))
            {
                throw new InvalidOperationException(
                    "Chưa cấu hình JwtSettings:SecretKey.");
            }

            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            if (string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException(
                    "Thiếu JwtSettings:Issuer hoặc Audience.");
            }

            var secretKey = Encoding.UTF8.GetBytes(secretKeyText);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(secretKey);
            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var expiryMinutes = double.Parse(
                jwtSettings["ExpiryMinutes"] ?? "60");

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var email = dto.Email?.Trim();

            if (string.IsNullOrWhiteSpace(email))
                return;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            // Không tiết lộ email có tồn tại trong hệ thống.
            if (user == null || !user.IsActive)
                return;

            var now = DateTime.UtcNow;

            var latest = await _context.PasswordResetCodes
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            // Chỉ cho phép gửi lại sau 60 giây.
            if (latest != null &&
                latest.CreatedAt > now.AddSeconds(-60))
                return;

            var oldCodes = await _context.PasswordResetCodes
                .Where(x => x.UserId == user.Id && x.UsedAt == null)
                .ToListAsync();

            foreach (var oldCode in oldCodes)
            {
                oldCode.UsedAt = now;
            }

            var otp = RandomNumberGenerator
                .GetInt32(0, 1_000_000)
                .ToString("D6");

            var resetCode = new PasswordResetCode
            {
                UserId = user.Id,
                CodeHash = BCrypt.Net.BCrypt.HashPassword(otp),
                CreatedAt = now,
                ExpiresAt = now.AddMinutes(5),
                Attempts = 0
            };

            _context.PasswordResetCodes.Add(resetCode);
            await _context.SaveChangesAsync();

            // Gửi OTP đến Gmail thật.
            await _emailService.SendOtpAsync(user.Email, otp);
        }

        public async Task<VerifyResetCodeResponseDto?>
            VerifyResetCodeAsync(VerifyResetCodeDto dto)
        {
            var email = dto.Email?.Trim();

            if (string.IsNullOrWhiteSpace(email))
                return null;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !user.IsActive)
                return null;

            var now = DateTime.UtcNow;

            var resetCode = await _context.PasswordResetCodes
                .Where(x =>
                    x.UserId == user.Id &&
                    x.UsedAt == null &&
                    x.ExpiresAt > now &&
                    x.Attempts < 5)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (resetCode == null)
                return null;

            resetCode.Attempts++;

            bool valid =
                !string.IsNullOrWhiteSpace(dto.Code) &&
                BCrypt.Net.BCrypt.Verify(
                    dto.Code,
                    resetCode.CodeHash);

            if (!valid)
            {
                await _context.SaveChangesAsync();
                return null;
            }

            resetCode.UsedAt = now;

            // Tạo Reset Token ngẫu nhiên.
            var resetToken = Convert.ToHexString(
                RandomNumberGenerator.GetBytes(32));

            // Chỉ lưu hash của token trong SQL Server.
            resetCode.ResetTokenHash = Convert.ToHexString(
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(resetToken)));

            resetCode.ResetTokenExpiresAt =
                now.AddMinutes(10);

            await _context.SaveChangesAsync();

            return new VerifyResetCodeResponseDto
            {
                Message = "Xác minh OTP thành công.",
                ResetToken = resetToken
            };
        }

        public async Task<bool> ResetPasswordAsync(
            ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.ResetToken) ||
                string.IsNullOrWhiteSpace(dto.NewPassword) ||
                dto.NewPassword.Length < 8 ||
                dto.NewPassword != dto.ConfirmPassword)
            {
                return false;
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Email == dto.Email.Trim());

            if (user == null || !user.IsActive)
                return false;

            var tokenHash = Convert.ToHexString(
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(dto.ResetToken)));

            var now = DateTime.UtcNow;

            var resetCode = await _context.PasswordResetCodes
                .FirstOrDefaultAsync(x =>
                    x.UserId == user.Id &&
                    x.ResetTokenHash == tokenHash &&
                    x.ResetTokenExpiresAt > now);

            if (resetCode == null)
                return false;

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            // Vô hiệu hóa Reset Token sau khi đổi mật khẩu.
            resetCode.ResetTokenHash = null;
            resetCode.ResetTokenExpiresAt = null;

            await _context.SaveChangesAsync();

            return true;
        }


    }
}