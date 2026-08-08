using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly AppDbContext _context;

    public SubjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Subject>> GetAllAsync()
    {
        return await _context.Subjects
            .OrderBy(subject => subject.Id)
            .ToListAsync();
    }

    public async Task<Subject?> GetByIdAsync(int id)
    {
        return await _context.Subjects
            .FirstOrDefaultAsync(subject => subject.Id == id);
    }

    public async Task<Subject> AddAsync(Subject subject)
    {
        _context.Subjects.Add(subject);

        await _context.SaveChangesAsync();

        return subject;
    }

    public async Task<Subject?> UpdateAsync(Subject subject)
    {
        var existingSubject = await _context.Subjects
            .FirstOrDefaultAsync(
                existing => existing.Id == subject.Id
            );

        if (existingSubject is null)
        {
            return null;
        }

        existingSubject.Code = subject.Code;
        existingSubject.Name = subject.Name;
        existingSubject.Description = subject.Description;

        await _context.SaveChangesAsync();

        return existingSubject;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(subject => subject.Id == id);

        if (subject is null)
        {
            return false;
        }

        _context.Subjects.Remove(subject);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _context.Subjects
            .AnyAsync(subject => subject.Code == code);
    }
}