using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IAssignmentRepository
{
    Task<List<Assignment>> GetAllAsync();
    Task<Assignment?> GetByIdAsync(int id);
    Task<Assignment?> GetByIdWithDetailsAsync(int id);
    Task<List<Assignment>> GetByTeacherIdAsync(int teacherId);
    Task<Assignment> AddAsync(Assignment assignment);
    Task<Assignment?> UpdateAsync(Assignment assignment);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByCodeAsync(string code);
}