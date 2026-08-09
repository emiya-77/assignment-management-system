using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly AppDbContext _context;

    public AssignmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Assignment>> GetAllAsync()
    {
        return await _context.Assignments
            .Include(a => a.TeacherAssignment)
                .ThenInclude(ta => ta.Teacher)
            .Include(a => a.TeacherAssignment)
                .ThenInclude(ta => ta.Course)
            .Include(a => a.TeacherAssignment)
                .ThenInclude(ta => ta.Subject)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Assignment?> GetByIdAsync(int id)
    {
        return await _context.Assignments
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Assignment?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Assignments
            .Include(a => a.TeacherAssignment)
                .ThenInclude(ta => ta.Teacher)
            .Include(a => a.TeacherAssignment)
                .ThenInclude(ta => ta.Course)
            .Include(a => a.TeacherAssignment)
                .ThenInclude(ta => ta.Subject)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Assignment>> GetByTeacherIdAsync(int teacherId)
    {
        return await _context.Assignments
            .Include(a => a.TeacherAssignment)
                .ThenInclude(ta => ta.Teacher)
            .Include(a => a.TeacherAssignment)
                .ThenInclude(ta => ta.Course)
            .Include(a => a.TeacherAssignment)
                .ThenInclude(ta => ta.Subject)
            .Where(a => a.TeacherAssignment.TeacherId == teacherId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Assignment> AddAsync(Assignment assignment)
    {
        _context.Assignments.Add(assignment);

        await _context.SaveChangesAsync();

        return assignment;
    }

    public async Task<Assignment?> UpdateAsync(Assignment assignment)
    {
        var existingAssignment = await _context.Assignments
            .FirstOrDefaultAsync(a => a.Id == assignment.Id);

        if (existingAssignment is null)
        {
            return null;
        }

        existingAssignment.Code = assignment.Code;
        existingAssignment.Title = assignment.Title;
        existingAssignment.Description = assignment.Description;
        existingAssignment.Deadline = assignment.Deadline;
        existingAssignment.MaximumMarks = assignment.MaximumMarks;
        existingAssignment.AllowSubmissionUpdate =
            assignment.AllowSubmissionUpdate;
        existingAssignment.IsPublished = assignment.IsPublished;
        existingAssignment.UpdatedAt = assignment.UpdatedAt;

        await _context.SaveChangesAsync();

        return existingAssignment;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var assignment = await _context.Assignments
            .FirstOrDefaultAsync(a => a.Id == id);

        if (assignment is null)
        {
            return false;
        }

        _context.Assignments.Remove(assignment);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _context.Assignments
            .AnyAsync(a => a.Code == code);
    }
}