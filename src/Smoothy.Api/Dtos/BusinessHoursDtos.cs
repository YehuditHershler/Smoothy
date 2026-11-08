using Smoothy.Domain.Entities;

namespace Smoothy.Api.Dtos;

/// <summary>
///
/// 
/// </summary> 

public record BusinessHoursDto(DayOfWeek DayOfWeek, TimeOnly StartTime, TimeOnly EndTime)
{

    public static BusinessHoursDto FromEntity(BusinessHours bh) =>
        new(bh.DayOfWeek, bh.StartTime, bh.EndTime);
}

public record UpdateBusinessHoursRequest(List<BusinessHoursDto> Hours);