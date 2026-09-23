namespace BookManager.Tests.Services.Auth;

public abstract class AuthServiceBase
{
    protected readonly IUserRepository _userRepository;
    protected readonly IJwtService _jwtService;
    protected readonly AuthService _authService;

    protected AuthServiceBase()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _jwtService = Substitute.For<IJwtService>();
        _authService = new AuthService(_userRepository, _jwtService);
    }
}
