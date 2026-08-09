using Application.DTOs.Assignments;

namespace Application.Interfaces.Services;

public interface IAssignmentService
{
    Task<List<AssignmentResponse>> GetAllAsync();

    Task<List<AssignmentResponse>> GetByTeacherIdAsync(
        int teacherId
    );

    Task<AssignmentResponse?> GetByIdAsync(int id);

    Task<AssignmentResponse> CreateAsync(
        int teacherId,
        CreateAssignmentRequest request
    );

    Task<AssignmentResponse?> UpdateAsync(
        int id,
        int teacherId,
        UpdateAssignmentRequest request
    );

    Task<bool> DeleteAsync(
        int id,
        int teacherId
    );

    Task<AssignmentResponse?> PublishAsync(
        int id,
        int teacherId
    );
}