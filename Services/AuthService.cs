using LabsTRVD.Data;
using LabsTRVD.DTOs.AuthDTOs;
using LabsTRVD.Entities;
using LabsTRVD.Interfaces.Services;
using LabsTRVD.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LabsTRVD.Services;

public class AuthService : IAuthService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IRepository<User> _userRepository;
    private readonly ApplicationDbContext _context;

    public AuthService(
        IOptions<JwtSettings> jwtSettings,
        IRepository<User> userRepository,
        ApplicationDbContext context)
    {
        _jwtSettings = jwtSettings.Value;
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<AuthResultDto> SignInAsync(SignInDto dto)
    {
        var users = await _userRepository.FindAsync(u => u.Email == dto.Email);
        var user = users.FirstOrDefault()
            ?? throw new UnauthorizedAccessException("Невірний email або пароль.");

        if (user.IsBlocked)
            throw new UnauthorizedAccessException("Ваш обліковий запис заблокований адміністратором. Зв'яжіться з підтримкою.");

        bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!isValid)
            throw new UnauthorizedAccessException("Невірний email або пароль.");

        user.LastLoginAt = DateTime.Now;
        await _userRepository.UpdateAsync(user);

        return await GenerateTokenPairAsync(user);
    }

    public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
    {
        var principal = GetPrincipalFromExpiredToken(dto.AccessToken);

        var userIdStr = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new SecurityTokenException("Невалідний access token.");

        if (!Guid.TryParse(userIdStr, out var userId))
            throw new SecurityTokenException("Невалідний формат userId.");

        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken && rt.UserId == userId);

        if (storedToken == null || !storedToken.IsActive)
            throw new SecurityTokenException("Refresh token недійсний або прострочений.");

        if (storedToken.User.IsBlocked)
            throw new UnauthorizedAccessException("Ваш обліковий запис заблокований.");

        storedToken.IsRevoked = true;
        _context.RefreshTokens.Update(storedToken);

        return await GenerateTokenPairAsync(storedToken.User);
    }

    public async Task RevokeTokenAsync(RevokeTokenRequestDto dto)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken);

        if (token == null || !token.IsActive)
            throw new SecurityTokenException("Токен не знайдено або вже відкликано.");

        token.IsRevoked = true;
        await _context.SaveChangesAsync();
    }
    private async Task<AuthResultDto> GenerateTokenPairAsync(User user)
    {
        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        var refreshExpires = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        _context.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.UserId,
            ExpiresAt = refreshExpires,
            CreatedAt = DateTime.Now,
            IsRevoked = false
        });

        await _context.SaveChangesAsync();

        return new AuthResultDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpires = DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
            RefreshTokenExpires = refreshExpires,
            Email = user.Email,
            Role = user.Role
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var validationParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = false
        };

        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, validationParams, out var secToken);

        if (secToken is not JwtSecurityToken jwtToken ||
       !jwtToken.Header.Alg.Equals("HS256", StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityTokenException("Невалідний access token.");
        }

        return principal;
    }
}