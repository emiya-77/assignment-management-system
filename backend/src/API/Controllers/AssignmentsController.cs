using API.Extensions;
using Application.DTOs.Assignments;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentsController(
        IAssignmentService assignmentService
    )
    {
        _assignmentService = assignmentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AssignmentResponse>>> GetAll()
    {
        if (User.IsInRole(Roles.Admin))
        {
            var assignments =
                await _assignmentService.GetAllAsync();

            return Ok(assignments);
        }

        if (User.IsInRole(Roles.Teacher))
        {
            var teacherId = User.GetUserId();

            var assignments =
                await _assignmentService
                    .GetByTeacherIdAsync(teacherId);

            return Ok(assignments);
        }

        return Forbid();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AssignmentResponse>> GetById(int id)
    {
        var assignment =
            await _assignmentService.GetByIdAsync(id);

        if (assignment is null)
        {
            return NotFound(new
            {
                message = "Assignment not found."
            });
        }

        if (User.IsInRole(Roles.Admin))
        {
            return Ok(assignment);
        }

        if (User.IsInRole(Roles.Teacher))
        {
            var teacherId = User.GetUserId();

            if (assignment.TeacherId != teacherId)
            {
                return Forbid();
            }

            return Ok(assignment);
        }

        return Forbid();
    }

    [HttpPost]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<ActionResult<AssignmentResponse>> Create(
        CreateAssignmentRequest request
    )
    {
        var teacherId = User.GetUserId();

        var assignment =
            await _assignmentService.CreateAsync(
                teacherId,
                request
            );

        return CreatedAtAction(
            nameof(GetById),
            new { id = assignment.Id },
            assignment
        );
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<ActionResult<AssignmentResponse>> Update(
        int id,
        UpdateAssignmentRequest request
    )
    {
        var teacherId = User.GetUserId();

        var assignment =
            await _assignmentService.UpdateAsync(
                id,
                teacherId,
                request
            );

        if (assignment is null)
        {
            return NotFound(new
            {
                message = "Assignment not found."
            });
        }

        return Ok(assignment);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<IActionResult> Delete(int id)
    {
        var teacherId = User.GetUserId();

        var deleted =
            await _assignmentService.DeleteAsync(
                id,
                teacherId
            );

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Assignment not found."
            });
        }

        return NoContent();
    }

    [HttpPatch("{id:int}/publish")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<ActionResult<AssignmentResponse>> Publish(int id)
    {
        var teacherId = User.GetUserId();

        var assignment =
            await _assignmentService.PublishAsync(
                id,
                teacherId
            );

        if (assignment is null)
        {
            return NotFound(new
            {
                message = "Assignment not found."
            });
        }

        return Ok(assignment);
    }
}