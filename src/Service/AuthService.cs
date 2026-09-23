using BookManager.DTOs.Auth;
using BookManager.Models;
using BookManager.Repositories;

namespace BookManager.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthUserDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
    {
        var usernameExists = await _userRepository.UsernameExistsAsync(dto.Username, ct);
        if (usernameExists)
            throw new InvalidOperationException("Username already exists.");

        var user = new User
        {
            Username = dto.Username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "User"
        };

        var createdUser = await _userRepository.AddAsync(user, ct);
        return MapToUserDto(createdUser);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username, ct);
        if (user is null)
            return null;

        var validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!validPassword)
            return null;

        return new AuthResponseDto
        {
            AccessToken = _jwtService.GenerateToken(user),
            RefreshToken = _jwtService.GenerateRefreshToken(user),
            User = MapToUserDto(user)
        };
    }

    private static AuthUserDto MapToUserDto(User user)
    {
        return new AuthUserDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role
        };
    }

    public async Task<AuthResponseDto?> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var principal = _jwtService.ValidateRefreshToken(refreshToken);
        if (principal is null)
            return null;

        var username = principal.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username))
            return null;

        var user = await _userRepository.GetByUsernameAsync(username, ct);
        if (user is null)
            return null;

        return new AuthResponseDto
        {
            AccessToken = _jwtService.GenerateToken(user),
            RefreshToken = _jwtService.GenerateRefreshToken(user),
            User = MapToUserDto(user)
        };
    }
}