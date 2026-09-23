using BookManager.Data;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Tests.Repositories;

public abstract class RepositoryTestBase : IDisposable
{
    protected readonly AppDbContext _context;

    protected RepositoryTestBase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
