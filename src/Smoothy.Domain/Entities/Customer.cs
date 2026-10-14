namespace Smoothy.Domain.Entities;

/// <summary>כרטיס לקוחה, כולל פרטי קשר והיסטוריה (דרך Appointments).</summary>
public class Customer : TenantEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }

    public List<Appointment> Appointments { get; set; } = [];
}