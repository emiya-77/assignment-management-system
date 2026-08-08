using Application.DTOs.Subjects;

namespace Application.Interfaces.Services;

public interface ISubjectService
{
    Task<List<SubjectResponse>> GetAllAsync();
    Task<SubjectResponse?> GetByIdAsync(int id);
    Task<SubjectResponse> CreateAsync(
        CreateSubjectRequest request
    );
    Task<SubjectResponse?> UpdateAsync(
        int id,
        UpdateSubjectRequest request
    );
    Task<bool> DeleteAsync(int id);
}