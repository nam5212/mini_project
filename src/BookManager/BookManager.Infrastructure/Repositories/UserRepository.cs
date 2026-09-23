using BookManager.Infrastructure.Data;
using BookManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using BookManager.Application.Interfaces;
namespace BookManager.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken ct = default)
    {
        return await _context.Users.AnyAsync(x => x.Username == username, ct);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Username == username, ct);
    }

    public async Task<User> AddAsync(User user, CancellationToken ct = default)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);
        return user;
    }
}
