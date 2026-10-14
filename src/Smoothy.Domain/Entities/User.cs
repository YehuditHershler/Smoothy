namespace Smoothy.Domain.Entities;
 
/// <summary>
/// משתמש מחובר (בעלת עסק או לקוחה). הסיסמה עצמה תנוהל בהמשך על ידי
/// ASP.NET Core Identity - כאן רק השדות העסקיים שלנו.
/// </summary>
public class User : TenantEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
 
    // אם זו משתמשת מסוג Customer, השדה הזה מצביע על כרטיס הלקוחה המלא שלה
    public Guid? CustomerId { get; set; }
 
    public Business Business { get; set; } = null!;
}
 