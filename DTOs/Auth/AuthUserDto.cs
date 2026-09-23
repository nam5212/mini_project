namespace BookManager.DTOs.Auth;

public class AuthUserDto
{
    public int Id { get; set; }

    public string Username { get; set; } = "";

    public string Role { get; set; } = "";
}