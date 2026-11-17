using Smoothy.Domain.Entities;

namespace Smoothy.Api.Dtos;

/// <summary>
///
/// 
/// </summary> 

public record BlockedTimeDto(Guid Id, DateTimeOffset StartTime, DateTimeOffset EndTime, string? Reason)
{

    public static BlockedTimeDto FromEntity(BlockedTime bt) =>
        new(bt.Id, bt.StartTime, bt.EndTime, bt.Reason);
}

public record CreateBlockedTimeRequest(DateTimeOffset StartTime, DateTimeOffset EndTime, string? Reason);
