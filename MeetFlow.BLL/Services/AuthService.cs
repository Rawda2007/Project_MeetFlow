using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
// "Task" alone is ambiguous whenever a file imports MeetFlow_DAL.Entities (which has its
// own "Task" entity) AND uses async Task methods. This alias fixes it for this file.
using Task = System.Threading.Tasks.Task;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MeetFlow.BLL.DTOs.Auth;
using MeetFlow.BLL.Interfaces;
using MeetFlow_DAL.Entities;
using MeetFlow_DAL.Repositories;

namespace MeetFlow.BLL.Services
{
    // Notice: no "using Microsoft.EntityFrameworkCore;" anywhere in this file.
    // The BLL only talks to IUnitOfWork / repository interfaces — it has no idea
    // EF Core exists. That's the whole point of the Repository + Unit of Work layer.
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration config, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _emailService = emailService;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            var emailExists = await _unitOfWork.Users.EmailExistsAsync(dto.Email);
            if (emailExists)
                throw new InvalidOperationException("This email is already registered.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return await IssueTokensAsync(user);
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);

            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            return await IssueTokensAsync(user);
        }

        public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            var existingToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(dto.RefreshToken);

            if (existingToken is null || existingToken.RevokedAt is not null || existingToken.ExpiresAt <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            // Rotate: revoke the old refresh token and issue a brand new pair.
            existingToken.RevokedAt = DateTime.UtcNow;

            var result = await IssueTokensAsync(existingToken.User);
            await _unitOfWork.SaveChangesAsync();

            return result;
        }

        public async Task LogoutAsync(int userId, RefreshTokenRequestDto dto)
        {
            var existingToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(dto.RefreshToken);

            // Logging out with an already-invalid/unknown token is a no-op, not an error —
            // the end state the caller wants (token not usable) is already true.
            if (existingToken is null || existingToken.RevokedAt is not null)
                return;

            // Make sure the token being revoked actually belongs to the caller —
            // otherwise an authenticated user could log someone else out by guessing their token.
            if (existingToken.UserId != userId)
                throw new UnauthorizedAccessException("This refresh token does not belong to you.");

            existingToken.RevokedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task LogoutAllAsync(int userId)
        {
            var activeTokens = await _unitOfWork.RefreshTokens.GetActiveByUserIdAsync(userId);
            foreach (var token in activeTokens)
                token.RevokedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);

            // Don't reveal whether the email exists — always respond the same way from the controller.
            if (user is null)
                return;

            var code = GenerateNumericCode(6);

            await _unitOfWork.PasswordResetCodes.AddAsync(new PasswordResetCode
            {
                UserId = user.Id,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.SaveChangesAsync();
            await _emailService.SendPasswordResetCodeAsync(user.Email, user.FullName, code);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
            if (user is null)
                throw new InvalidOperationException("Invalid code.");

            var resetCode = await _unitOfWork.PasswordResetCodes.GetValidCodeAsync(user.Id, dto.Code);
            if (resetCode is null)
                throw new InvalidOperationException("Invalid or expired code.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            resetCode.IsUsed = true;

            await _unitOfWork.SaveChangesAsync();
        }

        // ---------- helpers ----------

        private async Task<AuthResultDto> IssueTokensAsync(User user)
        {
            var accessTokenMinutes = int.Parse(_config["Jwt:AccessTokenExpiryMinutes"] ?? "60");
            var refreshTokenDays = int.Parse(_config["Jwt:RefreshTokenExpiryDays"] ?? "7");

            var (accessToken, accessTokenExpiresAt) = GenerateAccessToken(user, accessTokenMinutes);
            var refreshTokenValue = GenerateRefreshTokenString();
            var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays);

            await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAt = refreshTokenExpiresAt,
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.SaveChangesAsync();

            return new AuthResultDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessTokenExpiresAt,
                RefreshToken = refreshTokenValue,
                RefreshTokenExpiresAt = refreshTokenExpiresAt
            };
        }

        private (string token, DateTime expiresAt) GenerateAccessToken(User user, int expiryMinutes)
        {
            var jwtKey = _config["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key is not configured in appsettings.json.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("fullName", user.FullName)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }

        private static string GenerateRefreshTokenString()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private static string GenerateNumericCode(int length)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var number = BitConverter.ToUInt32(bytes, 0) % (uint)Math.Pow(10, length);
            return number.ToString(new string('0', length));
        }
    }
}
