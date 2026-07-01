using Microsoft.AspNetCore.Mvc;
using XaaDirApi.Models;
using XaaDirApi.Repositories;

namespace XaaDirApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthRepository _authRepository;
    public AuthController(AuthRepository authRepository) => _authRepository = authRepository;

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password)) return BadRequest("Username and password are required.");
        var user = _authRepository.Login(request);
        return user == null ? Unauthorized("Invalid username or password.") : Ok(user);
    }
}
