using BookManager.Infrastructure.Repositories;
using BookManager.Application.Interfaces;
using BookManager.Application.Services;
using BookManager.Application.Interfaces;

namespace BookManager.API.Extensions;
using BookManager.Infrastructure.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}
