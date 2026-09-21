using BookManager.Data;
using BookManager.DTOs.Auth;
using BookManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookManager.Services;

namespace BookManager.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly AppDbContext _context;
    public AuthController(AppDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
        
    }


    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto, CancellationToken ct)
    {   
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);
 
        var exists = await _context.Users
            .AnyAsync(x => x.Username == dto.Username, ct);
        
        if (exists)
            return BadRequest(new { message = "Username already exists." });

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "User"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Role
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Username == dto.Username, ct);

        if (user == null)
            return Unauthorized(new { message = "Invalid username or password." });

        var validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!validPassword)
            return Unauthorized(new { message = "Invalid username or password." });
 
        var token = _jwtService.GenerateToken(user);
 
        return Ok(new
        {
            accessToken = token,
            user = new
            {
                user.Id,
                user.Username,
                user.Role
            }
        });
    }
}