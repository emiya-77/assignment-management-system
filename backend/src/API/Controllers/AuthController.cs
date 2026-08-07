using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request
    )
    {
        var result = await _authService.LoginAsync(request);

        if (result is null)
        {
            return Unauthorized(
                new
                {
                    message = "Invalid email or password."
                }
            );
        }

        return Ok(result);
    }
}