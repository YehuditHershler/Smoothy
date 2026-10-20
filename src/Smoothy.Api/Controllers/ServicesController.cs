using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smoothy.Api.Dtos;
using Smoothy.Domain.Entities;
using Smoothy.Infrastructure;

namespace Smoothy.Api.Controllers;

[ApiController]
[Route("api/services")]
public class ServicesController(SmoothyDbContext db) : ControllerBase
{
    /// <summary>GET /api/services - כל השירותים הפעילים של העסק (מסונן אוטומטית לפי BusinessId).</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ServiceDto>>> GetAll()
    {
        var services = await db.Services
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();

        return Ok(services.Select(ServiceDto.FromEntity));
    }

    /// <summary>GET /api/services/{id} - שירות בודד.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ServiceDto>> GetById(Guid id)
    {
        var service = await db.Services.FindAsync(id);
        if (service is null) return NotFound();

        return Ok(ServiceDto.FromEntity(service));
    }

    /// <summary>
    /// POST /api/services - יצירת שירות חדש.
    /// הערה חשובה: ל-BusinessId עדיין אין כאן ערך אמיתי כי אין Auth,
    /// ולכן זמנית משתמשים ב-Guid קבוע לבדיקות. ברגע שיהיה Auth,
    /// השורה הזו תוחלף בקריאה ל-ICurrentBusinessProvider.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ServiceDto>> Create(CreateServiceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Name is required.");
        if (request.DurationMinutes <= 0)
            return BadRequest("DurationMinutes must be positive.");
        if (request.Price < 0)
            return BadRequest("Price cannot be negative.");

        var service = new Service
        {
            // TODO: להחליף ב-BusinessId אמיתי מתוך ה-Auth כשהוא יהיה קיים
            BusinessId = TestBusinessId,
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            DurationMinutes = request.DurationMinutes,
            Price = request.Price,
            LocationType = request.LocationType,
            TravelBufferMinutes = request.TravelBufferMinutes,
            IsActive = true,
        };

        db.Services.Add(service);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = service.Id }, ServiceDto.FromEntity(service));
    }

    /// <summary>Guid קבוע זמני, רק לבדיקות ידניות לפני שיש Auth אמיתי.</summary>
    public static readonly Guid TestBusinessId = Guid.Parse("11111111-1111-1111-1111-111111111111");
}