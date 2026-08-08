using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ISubjectRepository
{
    Task<List<Subject>> GetAllAsync();
    Task<Subject?> GetByIdAsync(int id);
    Task<Subject> AddAsync(Subject subject);
    Task<Subject?> UpdateAsync(Subject subject);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByCodeAsync(string code);
}