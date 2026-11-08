using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smoothy.Api.Dtos;
using Smoothy.Domain.Entities;
using Smoothy.Infrastructure;

namespace Smoothy.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController(SmoothyDbContext db, ICurrentBusinessProvider currentBusiness) : ControllerBase
{
    /// <summary>GET /api/customers - כל הלקוחות של העסק (מסונן אוטומטית לפי BusinessId).</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> GetAll(string? search)
    {
        var query = db.Customers.AsQueryable();

        if(!string.IsNullOrWhiteSpace(search))
        {
               query = query.Where(c => c.FullName.Contains(search)
                         || c.Phone.Contains(search)
                         || (c.Email != null && c.Email.Contains(search)));
        }
        var customers = await query.OrderBy(c => c.FullName).ToListAsync();
        return Ok(customers.Select(CustomerDto.FromEntity));
    }

    /// <summary>GET /api/customers/{id} - לקוח בודד.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id)
    {
        // var customer = await db.Customers.FindAsync(id);
        var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer is null) return NotFound();

        return Ok(CustomerDto.FromEntity(customer));
    }

    /// <summary>
    /// POST /api/customers - יצירת לקוח חדש.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerRequest request)
    {
        if (Validate(request.FullName, request.Phone) is { } error) return BadRequest(error);
        
        var businessId = currentBusiness.BusinessId;
        if (businessId is null) return Unauthorized();

        var customer = new Customer
        {
            BusinessId = businessId.Value,
            FullName = request.FullName,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            Notes = request.Notes
        };

        db.Customers.Add(customer);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, CustomerDto.FromEntity(customer));
    }

    /// <summary>
    /// PUT /api/customers/{id} - עדכון לקוח קיים.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Update(Guid id, UpdateCustomerRequest request)
    {

        if (Validate(request.FullName, request.Phone) is { } error)
            return BadRequest(error);

      //  var customer = await db.Customers.FindAsync(id);
        var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer is null) return NotFound();

        customer.FullName = request.FullName;
        customer.Phone = request.Phone;
        customer.Email = request.Email;
        customer.Address = request.Address;
        customer.Notes = request.Notes;
        customer.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Ok(CustomerDto.FromEntity(customer));
    }

      private static string? Validate(string fullName, string phone) =>
      string.IsNullOrWhiteSpace(fullName) ? "FullName is required."
    : string.IsNullOrWhiteSpace(phone) ? "Phone is required."
    : null;
}