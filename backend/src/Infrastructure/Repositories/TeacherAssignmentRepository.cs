using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TeacherAssignmentRepository
    : ITeacherAssignmentRepository
{
    private readonly AppDbContext _context;

    public TeacherAssignmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TeacherAssignment>> GetAllAsync()
    {
        return await _context.TeacherAssignments
            .Include(ta => ta.Teacher)
            .Include(ta => ta.Course)
            .Include(ta => ta.Subject)
            .OrderBy(ta => ta.Id)
            .ToListAsync();
    }

    public async Task<TeacherAssignment?> GetByIdAsync(int id)
    {
        return await _context.TeacherAssignments
            .Include(ta => ta.Teacher)
            .Include(ta => ta.Course)
            .Include(ta => ta.Subject)
            .FirstOrDefaultAsync(ta => ta.Id == id);
    }

    public async Task<TeacherAssignment> AddAsync(
        TeacherAssignment teacherAssignment
    )
    {
        _context.TeacherAssignments.Add(teacherAssignment);

        await _context.SaveChangesAsync();

        return teacherAssignment;
    }

    public async Task<bool> ExistsAsync(
        int teacherId,
        int courseId,
        int subjectId
    )
    {
        return await _context.TeacherAssignments
            .AnyAsync(ta =>
                ta.TeacherId == teacherId &&
                ta.CourseId == courseId &&
                ta.SubjectId == subjectId
            );
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var teacherAssignment =
            await _context.TeacherAssignments.FindAsync(id);

        if (teacherAssignment is null)
        {
            return false;
        }

        _context.TeacherAssignments.Remove(teacherAssignment);

        await _context.SaveChangesAsync();

        return true;
    }
}