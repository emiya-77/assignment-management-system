using API.Extensions;
using Application.DTOs.Users;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetAll()
    {
        var users = await _userService.GetAllAsync();

        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(
        CreateUserRequest request
    )
    {
        var user = await _userService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id },
            user
        );
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponse>> Update(
        int id,
        UpdateUserRequest request
    )
    {
        var user = await _userService.UpdateAsync(
            id,
            request
        );

        if (user is null)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        return Ok(user);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateUserStatusRequest request
    )
    {
        var updated = await _userService.UpdateStatusAsync(
            id,
            request.IsActive
        );

        if (!updated)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        return NoContent();
    }
}