using API.Extensions;
using Application.DTOs.Subjects;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SubjectResponse>>> GetAll()
    {
        var subjects = await _subjectService.GetAllAsync();

        return Ok(subjects);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectResponse>> GetById(int id)
    {
        var subject = await _subjectService.GetByIdAsync(id);

        if (subject is null)
        {
            return NotFound(new
            {
                message = "Subject not found."
            });
        }

        return Ok(subject);
    }

    [HttpPost]
    public async Task<ActionResult<SubjectResponse>> Create(
        CreateSubjectRequest request
    )
    {
        var subject = await _subjectService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = subject.Id },
            subject
        );
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SubjectResponse>> Update(
        int id,
        UpdateSubjectRequest request
    )
    {
        var subject = await _subjectService.UpdateAsync(
            id,
            request
        );

        if (subject is null)
        {
            return NotFound(new
            {
                message = "Subject not found."
            });
        }

        return Ok(subject);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _subjectService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Subject not found."
            });
        }

        return NoContent();
    }
}