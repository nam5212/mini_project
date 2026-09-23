using BookManager.Application.DTOs.Auth;

namespace BookManager.Application.Interfaces;

public interface IAuthService
{
    Task<AuthUserDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default);

    Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken ct = default);
    Task<AuthResponseDto?> RefreshAsync(string refreshToken, CancellationToken ct = default);
}
