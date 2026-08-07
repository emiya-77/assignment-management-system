using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;

    public CourseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Course>> GetAllAsync()
    {
        return await _context.Courses
            .OrderBy(course => course.Id)
            .ToListAsync();
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        return await _context.Courses
            .FirstOrDefaultAsync(course => course.Id == id);
    }

    public async Task<Course> AddAsync(Course course)
    {
        _context.Courses.Add(course);

        await _context.SaveChangesAsync();

        return course;
    }

    public async Task<Course?> UpdateAsync(Course course)
    {
        var existingCourse = await _context.Courses
            .FirstOrDefaultAsync(existing => existing.Id == course.Id);

        if (existingCourse is null)
        {
            return null;
        }

        existingCourse.Code = course.Code;
        existingCourse.Name = course.Name;
        existingCourse.Description = course.Description;

        await _context.SaveChangesAsync();

        return existingCourse;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(course => course.Id == id);

        if (course is null)
        {
            return false;
        }

        _context.Courses.Remove(course);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _context.Courses
            .AnyAsync(course => course.Code == code);
    }
}