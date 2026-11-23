using Smoothy.Domain.Entities;

namespace Smoothy.Api.Dtos;

public record RegisterBusinessRequest(string BusinessName, string FullName, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, Guid BusinessId, string FullName, string Email);
public record MeResponse(Guid Id, string FullName, string Email, Guid BusinessId, string Role);
