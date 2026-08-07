using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/test-auth")]
public class TestAuthorizationController : ControllerBase
{
    [HttpGet("public")]
    public IActionResult Public()
    {
        return Ok(new
        {
            message = "Anyone can access this endpoint."
        });
    }

    [Authorize]
    [HttpGet("authenticated")]
    public IActionResult Authenticated()
    {
        return Ok(new
        {
            message = "You are authenticated."
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult Admin()
    {
        return Ok(new
        {
            message = "You are an Admin."
        });
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("teacher")]
    public IActionResult Teacher()
    {
        return Ok(new
        {
            message = "You are a Teacher."
        });
    }

    [Authorize(Roles = "Student")]
    [HttpGet("student")]
    public IActionResult Student()
    {
        return Ok(new
        {
            message = "You are a Student."
        });
    }
}