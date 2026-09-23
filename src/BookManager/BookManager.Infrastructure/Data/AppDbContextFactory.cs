using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookManager.Infrastructure.Data;

// Design-time factory for EF Core tools. Reads the connection string from
// environment variables (ConnectionStrings__DefaultConnection) or falls
// back to a sensible default for local development.
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        // Use env var set by the user or docker compose; fallback to local default
        var connectionString = System.Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                               ?? "Host=localhost;Port=5432;Database=bookmanager;Username=postgres;Password=postgres123";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
