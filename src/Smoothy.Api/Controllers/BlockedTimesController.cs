using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smoothy.Api.Dtos;
using Smoothy.Domain.Entities;
using Smoothy.Infrastructure;


namespace Smoothy.Api.Controllers;

[ApiController]
[Route("api/blocked-times")]
public class BlockedTimesController(SmoothyDbContext db, ICurrentBusinessProvider currentBusiness) : ControllerBase
{
   [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BlockedTimeDto>>> GetAll(DateTimeOffset? from, DateTimeOffset? to)
    {

        var businessId = currentBusiness.BusinessId;
        if (businessId is null) return Unauthorized();

        var query = db.BlockedTimes.AsQueryable();

        if(from is not null)
           query = query.Where(bt => bt.StartTime >= from.Value.UtcDateTime);
        if(to is not null)
            query = query.Where(bt => bt.StartTime <= to.Value.UtcDateTime);
        

        var blockedTimes = await query
            .OrderBy(bt => bt.StartTime)
            .ToListAsync();

        return Ok(blockedTimes.Select(BlockedTimeDto.FromEntity));
    }

    [HttpPost]
    public async Task<ActionResult<BlockedTimeDto>> Create(CreateBlockedTimeRequest request)
    {
        var businessId = currentBusiness.BusinessId;
        if (businessId is null) return Unauthorized();

        if (request.StartTime >= request.EndTime)
            return BadRequest("Start time must be before end time.");

        if(request.Reason != null && request.Reason.Length > 200)
            return BadRequest("Reason must be less than 200 characters.");

var start = request.StartTime.UtcDateTime;
var end = request.EndTime.UtcDateTime;

// חפיפה: התור מתחיל לפני סוף החסימה ונגמר אחרי תחילתה. תורים מבוטלים לא נחשבים
var conflicts = await db.Appointments
    .Where(a => a.Status != AppointmentStatus.Cancelled
             && a.StartTime < end && a.EndTime > start)
    .OrderBy(a => a.StartTime)
    .Select(a => new { a.Id, a.StartTime, a.EndTime })
    .ToListAsync();

if (conflicts.Count > 0)
    return Conflict(new { message = "יש תורים בטווח הזמן הזה. יש להעביר או לבטל אותם לפני החסימה.", conflicts });



        var blockedTime = new BlockedTime
        {
            BusinessId = businessId.Value,
            StartTime = request.StartTime.UtcDateTime,
            EndTime = request.EndTime.UtcDateTime,
            Reason = request.Reason
        };
        db.BlockedTimes.Add(blockedTime);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = blockedTime.Id }, BlockedTimeDto.FromEntity(blockedTime));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var businessId = currentBusiness.BusinessId;
        if (businessId is null) return Unauthorized();

        var blockedTime = await db.BlockedTimes.FirstOrDefaultAsync(bt => bt.Id == id && bt.BusinessId == businessId.Value);
        if (blockedTime is null) return NotFound();

        db.BlockedTimes.Remove(blockedTime);
        await db.SaveChangesAsync();

        return NoContent();
    }

}