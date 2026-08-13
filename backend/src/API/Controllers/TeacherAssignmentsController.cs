using API.Extensions;
using Application.DTOs.TeacherAssignments;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeacherAssignmentsController : ControllerBase
{
    private readonly ITeacherAssignmentService
        _teacherAssignmentService;

    public TeacherAssignmentsController(
        ITeacherAssignmentService teacherAssignmentService
    )
    {
        _teacherAssignmentService =
            teacherAssignmentService;
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<
        ActionResult<List<TeacherAssignmentResponse>>
    > GetAll()
    {
        var teacherAssignments =
            await _teacherAssignmentService.GetAllAsync();

        return Ok(teacherAssignments);
    }

    [HttpGet("my-assignments")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<
        ActionResult<List<TeacherAssignmentResponse>>
    > GetMyAssignments()
    {
        var teacherId = User.GetUserId();

        var assignments =
            await _teacherAssignmentService
                .GetByTeacherIdAsync(teacherId);

        return Ok(assignments);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<
        ActionResult<TeacherAssignmentResponse>
    > GetById(int id)
    {
        var teacherAssignment =
            await _teacherAssignmentService.GetByIdAsync(id);

        if (teacherAssignment is null)
        {
            return NotFound(new
            {
                message = "Teacher assignment not found."
            });
        }

        return Ok(teacherAssignment);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<
        ActionResult<TeacherAssignmentResponse>
    > Create(
        CreateTeacherAssignmentRequest request
    )
    {
        var teacherAssignment =
            await _teacherAssignmentService.CreateAsync(
                request
            );

        return CreatedAtAction(
            nameof(GetById),
            new { id = teacherAssignment.Id },
            teacherAssignment
        );
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted =
            await _teacherAssignmentService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Teacher assignment not found."
            });
        }

        return NoContent();
    }
}