using BookManager.Models;

namespace BookManager.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken(User user);
    System.Security.Claims.ClaimsPrincipal? ValidateRefreshToken(string token);
}