using Smoothy.Domain.Entities;

namespace Smoothy.Api.Dtos;

/// <summary>
///
/// 
/// </summary> 

public record CustomerDto(
    Guid Id,
    string FullName,
    string Phone,
    string? Email,
    string? Address,
    string? Notes
    )
{
    public static CustomerDto FromEntity(Customer c) => new(
        c.Id, c.FullName, c.Phone, c.Email,
        c.Address, c.Notes);
}

/// <summary>מה שהלקוח שולח כדי ליצור לקוח חדש. בלי Id - הוא נוצר בשרת.</summary>
public record CreateCustomerRequest(
    string FullName,
    string Phone,
    string? Email,
    string? Address,
    string? Notes);

    public record UpdateCustomerRequest(
    string FullName,
    string Phone,
    string? Email,
    string? Address,
    string? Notes);