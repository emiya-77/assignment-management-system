using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SubmissionRepository
    : ISubmissionRepository
{
    private readonly AppDbContext _context;

    public SubmissionRepository(
        AppDbContext context
    )
    {
        _context = context;
    }

    public async Task<List<Submission>> GetAllAsync()
    {
        return await _context.Submissions
            .Include(s => s.Student)
            .Include(s => s.Assignment)
                .ThenInclude(a => a.TeacherAssignment)
                    .ThenInclude(ta => ta.Teacher)
            .Include(s => s.Assignment)
                .ThenInclude(a => a.TeacherAssignment)
                    .ThenInclude(ta => ta.Course)
            .Include(s => s.Assignment)
                .ThenInclude(a => a.TeacherAssignment)
                    .ThenInclude(ta => ta.Subject)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<Submission?> GetByIdAsync(int id)
    {
        return await _context.Submissions
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Submission?> GetByIdWithDetailsAsync(
        int id
    )
    {
        return await _context.Submissions
            .Include(s => s.Student)
            .Include(s => s.Assignment)
                .ThenInclude(a => a.TeacherAssignment)
                    .ThenInclude(ta => ta.Teacher)
            .Include(s => s.Assignment)
                .ThenInclude(a => a.TeacherAssignment)
                    .ThenInclude(ta => ta.Course)
            .Include(s => s.Assignment)
                .ThenInclude(a => a.TeacherAssignment)
                    .ThenInclude(ta => ta.Subject)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Submission>> GetByAssignmentIdAsync(
        int assignmentId
    )
    {
        return await _context.Submissions
            .Include(s => s.Student)
            .Include(s => s.Assignment)
                .ThenInclude(a => a.TeacherAssignment)
                    .ThenInclude(ta => ta.Course)
            .Include(s => s.Assignment)
                .ThenInclude(a => a.TeacherAssignment)
                    .ThenInclude(ta => ta.Subject)
            .Where(s => s.AssignmentId == assignmentId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<List<Submission>> GetByStudentIdAsync(
        int studentId
    )
    {
        return await _context.Submissions
            .Include(s => s.Student)
            .Include(s => s.Assignment)
                .ThenInclude(a => a.TeacherAssignment)
                    .ThenInclude(ta => ta.Teacher)
            .Include(s => s.Assignment)
                .ThenInclude(a => a.TeacherAssignment)
                    .ThenInclude(ta => ta.Course)
            .Include(s => s.Assignment)
                .ThenInclude(a => a.TeacherAssignment)
                    .ThenInclude(ta => ta.Subject)
            .Where(s => s.StudentId == studentId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(
        int assignmentId,
        int studentId
    )
    {
        return await _context.Submissions
            .AnyAsync(s =>
                s.AssignmentId == assignmentId &&
                s.StudentId == studentId
            );
    }

    public async Task<Submission> AddAsync(
        Submission submission
    )
    {
        _context.Submissions.Add(submission);

        await _context.SaveChangesAsync();

        return submission;
    }

    public async Task<Submission?> UpdateAsync(
        Submission submission
    )
    {
        var existingSubmission =
            await _context.Submissions
                .FirstOrDefaultAsync(s =>
                    s.Id == submission.Id
                );

        if (existingSubmission is null)
        {
            return null;
        }

        existingSubmission.Answer =
            submission.Answer;

        existingSubmission.Status =
            submission.Status;

        existingSubmission.Marks =
            submission.Marks;

        existingSubmission.Feedback =
            submission.Feedback;

        existingSubmission.UpdatedAt =
            submission.UpdatedAt;

        existingSubmission.GradedAt =
            submission.GradedAt;

        await _context.SaveChangesAsync();

        return existingSubmission;
    }
}