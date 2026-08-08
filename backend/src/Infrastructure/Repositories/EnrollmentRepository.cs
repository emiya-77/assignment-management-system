using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly AppDbContext _context;

    public EnrollmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudentCourse>> GetAllAsync()
    {
        return await _context.StudentCourses
            .Include(sc => sc.Student)
            .Include(sc => sc.Course)
            .OrderBy(sc => sc.StudentId)
            .ThenBy(sc => sc.CourseId)
            .ToListAsync();
    }

    public async Task<StudentCourse?> GetByIdsAsync(
        int studentId,
        int courseId
    )
    {
        return await _context.StudentCourses
            .Include(sc => sc.Student)
            .Include(sc => sc.Course)
            .FirstOrDefaultAsync(sc =>
                sc.StudentId == studentId &&
                sc.CourseId == courseId
            );
    }

    public async Task<StudentCourse> AddAsync(
        StudentCourse enrollment
    )
    {
        _context.StudentCourses.Add(enrollment);

        await _context.SaveChangesAsync();

        return enrollment;
    }

    public async Task<bool> ExistsAsync(
        int studentId,
        int courseId
    )
    {
        return await _context.StudentCourses
            .AnyAsync(sc =>
                sc.StudentId == studentId &&
                sc.CourseId == courseId
            );
    }

    public async Task<bool> DeleteAsync(
        int studentId,
        int courseId
    )
    {
        var enrollment =
            await _context.StudentCourses.FindAsync(
                studentId,
                courseId
            );

        if (enrollment is null)
        {
            return false;
        }

        _context.StudentCourses.Remove(enrollment);

        await _context.SaveChangesAsync();

        return true;
    }
}