namespace Smoothy.Domain.Entities;

/// <summary>
/// תור - הישות המרכזית במערכת.
/// שימו לב: DurationMinutes, TravelBufferMinutes ו-Price הם "Snapshot" -
/// מועתקים מה-Service בזמן היצירה, כדי שאם מחיר/משך השירות ישתנו בעתיד,
/// תורים ישנים לא ישתנו רטרואקטיבית.
/// </summary>
public class Appointment : TenantEntity
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public Guid? LocationId { get; set; }   // null אם השירות ניתן בבית הלקוחה
    public Location? Location { get; set; }

    public DateTime StartTime { get; set; }   // UTC
    public DateTime EndTime { get; set; }     // UTC

    // --- Snapshot בעת ההזמנה (ר' הערה למעלה) ---
    public int DurationMinutes { get; set; }
    public int TravelBufferMinutes { get; set; }
    public decimal Price { get; set; }
    public string? PriceBreakdown { get; set; }   // JSON: בסיס + תוספות

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Confirmed;
    public string? CancellationReason { get; set; }
}