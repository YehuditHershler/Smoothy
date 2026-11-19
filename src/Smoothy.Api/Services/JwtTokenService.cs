using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Smoothy.Infrastructure.Identity;

namespace Smoothy.Api.Services;

/// <summary>
/// ההגדרות נקראות מהסעיף "Jwt" בקונפיגורציה.
/// Issuer, Audience ו-ExpiryMinutes נמצאים ב-appsettings.json (לא סודי).
/// ה-Key הוא סוד, ונשמר ב-User Secrets (לא ב-git!).
/// </summary>
public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
}

public interface IJwtTokenService
{
    string CreateToken(ApplicationUser user);
}

/// <summary>
/// יוצר JWT חתום עבור משתמשת. ה-Token הוא "תעודת זהות" שהלקוח שולח בכל בקשה,
/// והשרת מאמת שהחתימה שלו תקפה, בלי לגשת ל-DB.
/// ה-BusinessId נכנס לתוך ה-Token (claim בשם business_id),
/// וכך ה-Controllers יודעים לאיזה עסק המשתמשת שייכת.
/// </summary>
public class JwtTokenService(IOptions<JwtSettings> options) : IJwtTokenService
{
    private readonly JwtSettings _settings = options.Value;

    public string CreateToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new("sub", user.Id.ToString()),
            new("email", user.Email ?? string.Empty),
            new("name", user.FullName),
            new("business_id", user.BusinessId.ToString()),
            new("role", user.Role.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}