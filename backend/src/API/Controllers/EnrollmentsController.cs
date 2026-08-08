using API.Extensions;
using Application.DTOs.Enrollments;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(
        IEnrollmentService enrollmentService
    )
    {
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EnrollmentResponse>>> GetAll()
    {
        var enrollments =
            await _enrollmentService.GetAllAsync();

        return Ok(enrollments);
    }

    [HttpGet("{studentId:int}/{courseId:int}")]
    public async Task<ActionResult<EnrollmentResponse>> GetByIds(
        int studentId,
        int courseId
    )
    {
        var enrollment =
            await _enrollmentService.GetByIdsAsync(
                studentId,
                courseId
            );

        if (enrollment is null)
        {
            return NotFound(new
            {
                message = "Enrollment not found."
            });
        }

        return Ok(enrollment);
    }

    [HttpPost]
    public async Task<ActionResult<EnrollmentResponse>> Create(
        CreateEnrollmentRequest request
    )
    {
        var enrollment =
            await _enrollmentService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetByIds),
            new
            {
                studentId = enrollment.StudentId,
                courseId = enrollment.CourseId
            },
            enrollment
        );
    }

    [HttpDelete("{studentId:int}/{courseId:int}")]
    public async Task<IActionResult> Delete(
        int studentId,
        int courseId
    )
    {
        var deleted =
            await _enrollmentService.DeleteAsync(
                studentId,
                courseId
            );

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Enrollment not found."
            });
        }

        return NoContent();
    }
}