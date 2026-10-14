namespace Smoothy.Domain.Entities;
 
/// <summary>
/// שעות עבודה קבועות לפי יום בשבוע. אפשר שתהיה יותר משורה אחת לאותו יום
/// (לדוגמה הפסקת צהריים: 09:00-13:00 ו-16:00-19:00).
/// השעות נשמרות כ-TimeOnly (ללא תאריך) - הן חוזרות על עצמן כל שבוע.
/// </summary>
public class BusinessHours : TenantEntity
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}