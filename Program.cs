using System.Text;
using BookManager.Data;
using BookManager.Repositories;
using BookManager.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "http://localhost:8080",
                "https://localhost:5001")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IJwtService, JwtService>();

static string GetConfigurationValue(IConfiguration configuration, params string[] keys)
{
    foreach (var key in keys)
    {
        var value = configuration[key];
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value.Trim();
        }
    }

    return string.Empty;
}

var jwtSecret = GetConfigurationValue(
        builder.Configuration,
        "JWT_SECRET",
        "Jwt:Key",
        "Jwt__Key",
        "JWT_KEY")
    .Length > 0
    ? GetConfigurationValue(
        builder.Configuration,
        "JWT_SECRET",
        "Jwt:Key",
        "Jwt__Key",
        "JWT_KEY")
    : "ThisIsASecretKeyForBookManager123456";

var jwtIssuer = GetConfigurationValue(
        builder.Configuration,
        "JWT_ISSUER",
        "Jwt:Issuer",
        "Jwt__Issuer")
    .Length > 0
    ? GetConfigurationValue(
        builder.Configuration,
        "JWT_ISSUER",
        "Jwt:Issuer",
        "Jwt__Issuer")
    : "BookApi";

var jwtAudience = GetConfigurationValue(
        builder.Configuration,
        "JWT_AUDIENCE",
        "Jwt:Audience",
        "Jwt__Audience")
    .Length > 0
    ? GetConfigurationValue(
        builder.Configuration,
        "JWT_AUDIENCE",
        "Jwt:Audience",
        "Jwt__Audience")
    : "BookApiClient";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

var hasHttpsPort = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT"));
if (hasHttpsPort)
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
