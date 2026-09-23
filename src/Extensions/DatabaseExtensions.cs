using BookManager.Data;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = GetRequiredConfigurationValue(
            configuration,
            "ConnectionStrings:DefaultConnection",
            "ConnectionStrings__DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    public static WebApplication ApplyDatabaseMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();

        return app;
    }

    private static string GetRequiredConfigurationValue(IConfiguration configuration, params string[] keys)
    {
        foreach (var key in keys)
        {
            var value = configuration[key];
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
        }

        throw new InvalidOperationException($"Missing required configuration value. Set one of: {string.Join(", ", keys)}.");
    }
}
