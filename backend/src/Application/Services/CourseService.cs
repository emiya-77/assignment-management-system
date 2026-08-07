using Application.DTOs.Courses;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<List<CourseResponse>> GetAllAsync()
    {
        var courses = await _courseRepository.GetAllAsync();

        return courses
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<CourseResponse?> GetByIdAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        return course is null
            ? null
            : MapToResponse(course);
    }

    public async Task<CourseResponse> CreateAsync(
        CreateCourseRequest request
    )
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var codeExists =
            await _courseRepository.ExistsByCodeAsync(normalizedCode);

        if (codeExists)
        {
            throw new InvalidOperationException(
                "A course with this code already exists."
            );
        }

        var course = new Course
        {
            Code = normalizedCode,
            Name = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),
            IsActive = true
        };

        var createdCourse =
            await _courseRepository.AddAsync(course);

        return MapToResponse(createdCourse);
    }

    public async Task<CourseResponse?> UpdateAsync(
        int id,
        UpdateCourseRequest request
    )
    {
        var existingCourse =
            await _courseRepository.GetByIdAsync(id);

        if (existingCourse is null)
        {
            return null;
        }

        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (!string.Equals(
                existingCourse.Code,
                normalizedCode,
                StringComparison.OrdinalIgnoreCase
            ))
        {
            var codeExists =
                await _courseRepository.ExistsByCodeAsync(
                    normalizedCode
                );

            if (codeExists)
            {
                throw new InvalidOperationException(
                    "A course with this code already exists."
                );
            }
        }

        existingCourse.Code = normalizedCode;
        existingCourse.Name = request.Name.Trim();
        existingCourse.Description =
            string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();

        var updatedCourse =
            await _courseRepository.UpdateAsync(existingCourse);

        return updatedCourse is null
            ? null
            : MapToResponse(updatedCourse);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _courseRepository.DeleteAsync(id);
    }

    private static CourseResponse MapToResponse(Course course)
    {
        return new CourseResponse
        {
            Id = course.Id,
            Code = course.Code,
            Name = course.Name,
            Description = course.Description,
            IsActive = course.IsActive,
            CreatedAt = course.CreatedAt
        };
    }
}