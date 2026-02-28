using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PhytoIntellect.Application.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    private readonly IConfiguration _config = config;

    public string CreateAccessToken(User user)
    {
        // 1. تحضير الـ Claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Role, user.Role),
            // إضافة الـ Jti (Unique ID للتوكن نفسه) - ممارسة احترافية
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // 2. سحب البيانات من الـ AppSettings
        var keyStr = _config["JwtSettings:Key"];
        var issuer = _config["JwtSettings:Issuer"];
        var audience = _config["JwtSettings:Audience"];
        // سحب المدة (بالأيام) اللي اتفقنا نكبرها
        var durationInDays = double.Parse(_config["JwtSettings:DurationInDays"] ?? "7");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 3. بناء التوكن
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(durationInDays), // استخدام المدة الجديدة هنا
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }
}