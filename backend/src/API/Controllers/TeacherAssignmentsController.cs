using API.Extensions;
using Application.DTOs.TeacherAssignments;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
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
    public async Task<
        ActionResult<List<TeacherAssignmentResponse>>
    > GetAll()
    {
        var teacherAssignments =
            await _teacherAssignmentService.GetAllAsync();

        return Ok(teacherAssignments);
    }

    [HttpGet("{id:int}")]
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