using BookManager.Models;

namespace BookManager.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}