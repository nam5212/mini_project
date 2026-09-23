using BookManager.Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookManager.Application.Services;
using BookManager.Application.Interfaces;

namespace BookManager.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }


    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthUserDto>> Register([FromBody] RegisterDto dto, CancellationToken ct)
    {
        var user = await _authService.RegisterAsync(dto, ct);
        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(dto, ct);
        if (result is null)
            throw new UnauthorizedAccessException("Invalid username or password.");

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshRequestDto dto, CancellationToken ct)
    {
        var result = await _authService.RefreshAsync(dto.RefreshToken, ct);
        if (result is null)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        return Ok(result);
    }
}
