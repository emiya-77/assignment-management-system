using Application.DTOs.TeacherAssignments;

namespace Application.Interfaces.Services;

public interface ITeacherAssignmentService
{
    Task<List<TeacherAssignmentResponse>> GetAllAsync();
    Task<TeacherAssignmentResponse?> GetByIdAsync(int id);
    Task<TeacherAssignmentResponse> CreateAsync(
        CreateTeacherAssignmentRequest request
    );
    Task<bool> DeleteAsync(int id);
}