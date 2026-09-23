namespace BookManager.DTOs.Auth;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";

    public AuthUserDto User { get; set; } = new();
}