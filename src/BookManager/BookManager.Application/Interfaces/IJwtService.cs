using BookManager.Domain.Entities;

namespace BookManager.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken(User user);
    System.Security.Claims.ClaimsPrincipal? ValidateRefreshToken(string token);
}
