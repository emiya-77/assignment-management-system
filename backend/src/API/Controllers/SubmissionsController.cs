using API.Extensions;
using Application.DTOs.Submissions;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _submissionService;

    public SubmissionsController(
        ISubmissionService submissionService
    )
    {
        _submissionService = submissionService;
    }

    [HttpGet]
    public async Task<
        ActionResult<List<SubmissionResponse>>
    > GetAll()
    {
        if (User.IsInRole(Roles.Admin))
        {
            var submissions =
                await _submissionService.GetAllAsync();

            return Ok(submissions);
        }

        if (User.IsInRole(Roles.Student))
        {
            var studentId = User.GetUserId();

            var submissions =
                await _submissionService
                    .GetByStudentIdAsync(studentId);

            return Ok(submissions);
        }

        return Forbid();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubmissionResponse>>
        GetById(int id)
    {
        var submission =
            await _submissionService.GetByIdAsync(id);

        if (submission is null)
        {
            return NotFound(new
            {
                message = "Submission not found."
            });
        }

        if (User.IsInRole(Roles.Admin))
        {
            return Ok(submission);
        }

        var userId = User.GetUserId();

        if (
            User.IsInRole(Roles.Student) &&
            submission.StudentId == userId
        )
        {
            return Ok(submission);
        }

        if (
            User.IsInRole(Roles.Teacher) &&
            submission.TeacherId == userId
        )
        {
            return Ok(submission);
        }

        return Forbid();
    }

    [HttpGet("assignment/{assignmentId:int}")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<
        ActionResult<List<SubmissionResponse>>
    > GetByAssignmentId(int assignmentId)
    {
        var teacherId = User.GetUserId();

        var submissions =
            await _submissionService
                .GetByAssignmentIdAsync(
                    assignmentId,
                    teacherId
                );

        return Ok(submissions);
    }

    [HttpPost("assignment/{assignmentId:int}")]
    [Authorize(Roles = Roles.Student)]
    public async Task<ActionResult<SubmissionResponse>>
        Create(
            int assignmentId,
            CreateSubmissionRequest request
        )
    {
        var studentId = User.GetUserId();

        var submission =
            await _submissionService.CreateAsync(
                assignmentId,
                studentId,
                request
            );

        return CreatedAtAction(
            nameof(GetById),
            new { id = submission.Id },
            submission
        );
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Student)]
    public async Task<ActionResult<SubmissionResponse>>
        Update(
            int id,
            UpdateSubmissionRequest request
        )
    {
        var studentId = User.GetUserId();

        var submission =
            await _submissionService.UpdateAsync(
                id,
                studentId,
                request
            );

        if (submission is null)
        {
            return NotFound(new
            {
                message = "Submission not found."
            });
        }

        return Ok(submission);
    }

    [HttpPatch("{id:int}/grade")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<ActionResult<SubmissionResponse>>
        Grade(
            int id,
            GradeSubmissionRequest request
        )
    {
        var teacherId = User.GetUserId();

        var submission =
            await _submissionService.GradeAsync(
                id,
                teacherId,
                request
            );

        if (submission is null)
        {
            return NotFound(new
            {
                message = "Submission not found."
            });
        }

        return Ok(submission);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<ActionResult<SubmissionResponse>>
        UpdateStatus(
            int id,
            UpdateSubmissionStatusRequest request
        )
    {
        var teacherId = User.GetUserId();

        var submission =
            await _submissionService.UpdateStatusAsync(
                id,
                teacherId,
                request
            );

        if (submission is null)
        {
            return NotFound(new
            {
                message = "Submission not found."
            });
        }

        return Ok(submission);
    }
}