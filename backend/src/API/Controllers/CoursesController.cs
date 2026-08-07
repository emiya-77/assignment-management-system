using API.Extensions;
using Application.DTOs.Courses;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CourseResponse>>> GetAll()
    {
        var courses = await _courseService.GetAllAsync();

        return Ok(courses);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CourseResponse>> GetById(int id)
    {
        var course = await _courseService.GetByIdAsync(id);

        if (course is null)
        {
            return NotFound(new
            {
                message = "Course not found."
            });
        }

        return Ok(course);
    }

    [HttpPost]
    public async Task<ActionResult<CourseResponse>> Create(
        CreateCourseRequest request
    )
    {
        var course = await _courseService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = course.Id },
            course
        );
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CourseResponse>> Update(
        int id,
        UpdateCourseRequest request
    )
    {
        var course = await _courseService.UpdateAsync(
            id,
            request
        );

        if (course is null)
        {
            return NotFound(new
            {
                message = "Course not found."
            });
        }

        return Ok(course);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _courseService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Course not found."
            });
        }

        return NoContent();
    }
}