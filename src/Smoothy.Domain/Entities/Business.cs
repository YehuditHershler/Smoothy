namespace Smoothy.Domain.Entities;
 
/// <summary>
/// העסק עצמו (Tenant). זו הישות היחידה בלי BusinessId - היא ה"שורש" שכל השאר משויך אליו.
/// כרגע עסק אחד = בעלת עסק אחת, אבל השדה Users (ריבוי) משאיר מקום להרחבה.
/// </summary>
public class Business
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
 
    public List<User> Users { get; set; } = [];
}
