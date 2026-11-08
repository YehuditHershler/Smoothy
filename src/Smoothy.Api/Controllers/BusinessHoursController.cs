using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smoothy.Api.Dtos;
using Smoothy.Domain.Entities;
using Smoothy.Infrastructure;


namespace Smoothy.Api.Controllers;

[ApiController]
[Route("api/business-hours")]
public class BusinessHoursController(SmoothyDbContext db, ICurrentBusinessProvider currentBusiness) : ControllerBase
{
    /// <summary>GET /api/business-hours - שעות הפעילות של העסק (מסונן אוטומטית לפי BusinessId).</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BusinessHoursDto>>> Get()
    {
        var hours = await db.BusinessHours
            .OrderBy(h => h.DayOfWeek).ThenBy(h => h.StartTime)
            .ToListAsync();
        return Ok(hours.Select(BusinessHoursDto.FromEntity));
    }

    /// <summary>
    /// PUT /api/business-hours - עדכון שעות הפעילות של העסק.
    /// הערה חשובה: ל-BusinessId עדיין אין כאן ערך אמיתי כי אין Auth,
    /// ולכן זמנית משתמשים ב-Guid קבוע לבדיקות. ברגע שיהיה Auth,
    /// השורה הזו תוחלף בקריאה ל-ICurrentBusinessProvider.
    /// </summary>
    [HttpPut]   
    public async Task<ActionResult<IReadOnlyList<BusinessHoursDto>>> Replace(UpdateBusinessHoursRequest request)
    {
        var businessId = currentBusiness.BusinessId;
        if (businessId is null) return Unauthorized();

        if (request.Hours is null || request.Hours.Count == 0)
            return BadRequest("At least one working hours range is required.");

        foreach (var h in request.Hours)
        {
            if (h.StartTime >= h.EndTime)
                return BadRequest($"{h.DayOfWeek}: שעת הסיום חייבת להיות אחרי שעת ההתחלה.");
        }

        foreach (var day in request.Hours.GroupBy(h => h.DayOfWeek))
        {
            var sorted = day.OrderBy(h => h.StartTime).ToList();

            for (var i = 1; i < sorted.Count; i++)
            {
                // אם ההתחלה של הטווח הנוכחי לפני הסיום של הקודם, יש חפיפה
                if (sorted[i].StartTime < sorted[i - 1].EndTime)
                    return BadRequest($"{day.Key}: יש חפיפה בין טווחי שעות.");
            }
        }

        // מוחקים את הקיים ומוסיפים את החדש. SaveChanges אחד = הכול או כלום
        var existing = await db.BusinessHours.ToListAsync();
        db.BusinessHours.RemoveRange(existing);

        db.BusinessHours.AddRange(request.Hours.Select(h => new BusinessHours
        {
            BusinessId = businessId.Value,
            DayOfWeek = h.DayOfWeek,
            StartTime = h.StartTime,
            EndTime = h.EndTime,
        }));

        await db.SaveChangesAsync();
        return await Get();

    }
}