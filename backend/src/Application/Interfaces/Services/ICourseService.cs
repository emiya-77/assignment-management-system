using Application.DTOs.Courses;

namespace Application.Interfaces.Services;

public interface ICourseService
{
    Task<List<CourseResponse>> GetAllAsync();
    Task<CourseResponse?> GetByIdAsync(int id);
    Task<CourseResponse> CreateAsync(
        CreateCourseRequest request
    );
    Task<CourseResponse?> UpdateAsync(
        int id,
        UpdateCourseRequest request
    );
    Task<bool> DeleteAsync(int id);
}