using BookManager.Controllers;

namespace BookManager.Tests.Controllers.Auth;

public abstract class AuthControllerBase
{
    protected readonly IAuthService _authService;
    protected readonly AuthController _controller;
    protected readonly HttpContext _httpContext;

    protected AuthControllerBase()
    {
        _authService = Substitute.For<IAuthService>();
        _controller = new AuthController(_authService);

        _httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContext
        };
    }
}
