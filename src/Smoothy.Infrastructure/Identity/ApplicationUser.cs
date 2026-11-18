using Microsoft.AspNetCore.Identity;
using Smoothy.Domain.Entities;

namespace Smoothy.Infrastructure.Identity;

/// <summary>
/// משתמשת במערכת. יורשת מ-IdentityUser (שמטפל בסיסמה מוצפנת, אימייל וכו')
/// ומוסיפה את השדות העסקיים שלנו.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public Guid BusinessId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public Guid? CustomerId { get; set; }   // רק למשתמשת מסוג Customer
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}