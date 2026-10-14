namespace Smoothy.Domain.Entities;
 
/// <summary>
/// הגדרת שירות (לדוגמה "החלקה ראשונה"). זהו ה"קטלוג" - ה-Price וה-Duration כאן
/// יכולים להשתנות בעתיד, ולכן בעת יצירת תור שומרים Snapshot בתור עצמו (ר' Appointment).
/// </summary>
public class Service : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
 
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
 
    public ServiceLocationType LocationType { get; set; } = ServiceLocationType.AtBusiness;
 
    /// <summary>Buffer נסיעה קבוע בדקות, רלוונטי רק כש-LocationType כולל AtCustomer.</summary>
    public int? TravelBufferMinutes { get; set; }
 
    public bool IsActive { get; set; } = true;
}
 