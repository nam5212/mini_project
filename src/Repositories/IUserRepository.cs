using BookManager.Models;

namespace BookManager.Repositories;

public interface IUserRepository
{
    Task<bool> UsernameExistsAsync(string username, CancellationToken ct = default);

    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);

    Task<User> AddAsync(User user, CancellationToken ct = default);
}