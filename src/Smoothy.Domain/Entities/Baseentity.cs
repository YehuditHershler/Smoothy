namespace Smoothy.Domain.Entities;
 
/// <summary>
/// בסיס משותף לכל הישויות ששייכות לעסק מסוים (הכול חוץ מ-Business עצמו).
/// כל טבלה כזו תסונן אוטומטית לפי BusinessId ב-DbContext (Global Query Filter) –
/// כך עסק אחד לא יכול לראות נתונים של עסק אחר, גם אם יש טעות בקוד של שאילתה בודדת.
/// </summary>
public abstract class TenantEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BusinessId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
 