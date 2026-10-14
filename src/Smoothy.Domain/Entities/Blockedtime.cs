namespace Smoothy.Domain.Entities;
 
/// <summary>
/// חסימת זמן חד-פעמית ביומן (חופשה, פגישה אישית וכו').
/// בניגוד ל-BusinessHours, זה טווח תאריך-שעה קונקרטי, לא יום-בשבוע חוזר.
/// </summary>
public class BlockedTime : TenantEntity
{
    public DateTime StartTime { get; set; }   // UTC
    public DateTime EndTime { get; set; }     // UTC
    public string? Reason { get; set; }
}
 