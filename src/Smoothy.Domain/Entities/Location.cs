namespace Smoothy.Domain.Entities;

/// <summary>
/// מיקום מתן השירות. ב-MVP לרוב יהיה רשומה אחת מסוג "הסלון עצמו",
/// ושירות בבית הלקוחה לא דורש Location קבוע (הכתובת מגיעה מהלקוחה בזמן ההזמנה).
/// </summary>
public class Location : TenantEntity
{
    public string Name { get; set; } = string.Empty;     // לדוגמה: "הסלון"
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
}