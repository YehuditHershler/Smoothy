using Smoothy.Domain.Entities;

namespace Smoothy.Api.Dtos;

/// <summary>
/// מה שחוזר ללקוח כשמבקשים שירות/רשימת שירותים.
/// לא מחזירים את ה-Entity עצמו ישירות מה-API - כלל אצבע חשוב:
/// ה-DTO הוא "חוזה" ציבורי, וה-Entity יכול להשתנות פנימית בלי לשבור את ה-API.
/// </summary>
public record ServiceDto(
    Guid Id,
    string Name,
    string? Description,
    string? Category,
    int DurationMinutes,
    decimal Price,
    ServiceLocationType LocationType,
    int? TravelBufferMinutes,
    bool IsActive)
{
    public static ServiceDto FromEntity(Service s) => new(
        s.Id, s.Name, s.Description, s.Category,
        s.DurationMinutes, s.Price, s.LocationType,
        s.TravelBufferMinutes, s.IsActive);
}

/// <summary>מה שהלקוח שולח כדי ליצור שירות חדש. בלי Id - הוא נוצר בשרת.</summary>
public record CreateServiceRequest(
    string Name,
    string? Description,
    string? Category,
    int DurationMinutes,
    decimal Price,
    ServiceLocationType LocationType,
    int? TravelBufferMinutes);