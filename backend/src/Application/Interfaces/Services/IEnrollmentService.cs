using Application.DTOs.Enrollments;

namespace Application.Interfaces.Services;

public interface IEnrollmentService
{
    Task<List<EnrollmentResponse>> GetAllAsync();
    Task<EnrollmentResponse?> GetByIdsAsync(
        int studentId,
        int courseId
    );
    Task<EnrollmentResponse> CreateAsync(
        CreateEnrollmentRequest request
    );
    Task<bool> DeleteAsync(
        int studentId,
        int courseId
    );
}