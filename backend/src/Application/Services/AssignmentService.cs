using Application.DTOs.Assignments;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ITeacherAssignmentRepository
        _teacherAssignmentRepository;

    public AssignmentService(
        IAssignmentRepository assignmentRepository,
        ITeacherAssignmentRepository teacherAssignmentRepository
    )
    {
        _assignmentRepository = assignmentRepository;
        _teacherAssignmentRepository =
            teacherAssignmentRepository;
    }

    public async Task<List<AssignmentResponse>> GetAllAsync()
    {
        var assignments =
            await _assignmentRepository.GetAllAsync();

        return assignments
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<List<AssignmentResponse>> GetByTeacherIdAsync(
        int teacherId
    )
    {
        var assignments =
            await _assignmentRepository
                .GetByTeacherIdAsync(teacherId);

        return assignments
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<AssignmentResponse?> GetByIdAsync(int id)
    {
        var assignment =
            await _assignmentRepository
                .GetByIdWithDetailsAsync(id);

        return assignment is null
            ? null
            : MapToResponse(assignment);
    }

    public async Task<AssignmentResponse> CreateAsync(
        int teacherId,
        CreateAssignmentRequest request
    )
    {
        var normalizedCode =
            request.Code.Trim().ToUpperInvariant();

        var codeExists =
            await _assignmentRepository
                .ExistsByCodeAsync(normalizedCode);

        if (codeExists)
        {
            throw new InvalidOperationException(
                "An assignment with this code already exists."
            );
        }

        var teacherAssignment =
            await _teacherAssignmentRepository
                .GetByIdAsync(request.TeacherAssignmentId);

        if (teacherAssignment is null)
        {
            throw new KeyNotFoundException(
                "Teacher assignment not found."
            );
        }

        if (teacherAssignment.TeacherId != teacherId)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to create an assignment for this teacher assignment."
            );
        }

        var assignment = new Assignment
        {
            Code = normalizedCode,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Deadline = request.Deadline,
            MaximumMarks = request.MaximumMarks,
            AllowSubmissionUpdate =
                request.AllowSubmissionUpdate,
            TeacherAssignmentId =
                request.TeacherAssignmentId,
            IsPublished = false
        };

        var createdAssignment =
            await _assignmentRepository.AddAsync(assignment);

        var assignmentWithDetails =
            await _assignmentRepository
                .GetByIdWithDetailsAsync(createdAssignment.Id);

        return MapToResponse(assignmentWithDetails!);
    }

    public async Task<AssignmentResponse?> UpdateAsync(
        int id,
        int teacherId,
        UpdateAssignmentRequest request
    )
    {
        var assignment =
            await _assignmentRepository.GetByIdAsync(id);

        if (assignment is null)
        {
            return null;
        }

        var teacherAssignment =
            await _teacherAssignmentRepository
                .GetByIdAsync(assignment.TeacherAssignmentId);

        if (teacherAssignment is null)
        {
            throw new KeyNotFoundException(
                "Teacher assignment not found."
            );
        }

        if (teacherAssignment.TeacherId != teacherId)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to update this assignment."
            );
        }

        var normalizedCode =
            request.Code.Trim().ToUpperInvariant();

        if (!string.Equals(
                assignment.Code,
                normalizedCode,
                StringComparison.OrdinalIgnoreCase
            ))
        {
            var codeExists =
                await _assignmentRepository
                    .ExistsByCodeAsync(normalizedCode);

            if (codeExists)
            {
                throw new InvalidOperationException(
                    "An assignment with this code already exists."
                );
            }
        }

        assignment.Code = normalizedCode;
        assignment.Title = request.Title.Trim();
        assignment.Description = request.Description.Trim();
        assignment.Deadline = request.Deadline;
        assignment.MaximumMarks = request.MaximumMarks;
        assignment.AllowSubmissionUpdate =
            request.AllowSubmissionUpdate;
        assignment.UpdatedAt = DateTime.UtcNow;

        await _assignmentRepository.UpdateAsync(assignment);

        var updatedAssignment =
            await _assignmentRepository
                .GetByIdWithDetailsAsync(id);

        return updatedAssignment is null
            ? null
            : MapToResponse(updatedAssignment);
    }

    public async Task<bool> DeleteAsync(
        int id,
        int teacherId
    )
    {
        var assignment =
            await _assignmentRepository.GetByIdAsync(id);

        if (assignment is null)
        {
            return false;
        }

        var teacherAssignment =
            await _teacherAssignmentRepository
                .GetByIdAsync(assignment.TeacherAssignmentId);

        if (teacherAssignment is null)
        {
            throw new KeyNotFoundException(
                "Teacher assignment not found."
            );
        }

        if (teacherAssignment.TeacherId != teacherId)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to delete this assignment."
            );
        }

        return await _assignmentRepository.DeleteAsync(id);
    }

    public async Task<AssignmentResponse?> PublishAsync(
        int id,
        int teacherId
    )
    {
        var assignment =
            await _assignmentRepository.GetByIdAsync(id);

        if (assignment is null)
        {
            return null;
        }

        var teacherAssignment =
            await _teacherAssignmentRepository
                .GetByIdAsync(assignment.TeacherAssignmentId);

        if (teacherAssignment is null)
        {
            throw new KeyNotFoundException(
                "Teacher assignment not found."
            );
        }

        if (teacherAssignment.TeacherId != teacherId)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to publish this assignment."
            );
        }

        assignment.IsPublished = true;
        assignment.UpdatedAt = DateTime.UtcNow;

        await _assignmentRepository.UpdateAsync(assignment);

        var updatedAssignment =
            await _assignmentRepository
                .GetByIdWithDetailsAsync(id);

        return updatedAssignment is null
            ? null
            : MapToResponse(updatedAssignment);
    }

    public async Task<List<AssignmentResponse>> GetByStudentIdAsync(
        int studentId
    )
    {
        var assignments =
            await _assignmentRepository
                .GetByStudentIdAsync(studentId);

        return assignments
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<AssignmentResponse?> GetByIdForStudentAsync(
        int id,
        int studentId
    )
    {
        var assignment =
            await _assignmentRepository
                .GetByIdForStudentAsync(id, studentId);

        return assignment is null
            ? null
            : MapToResponse(assignment);
    }

    private static AssignmentResponse MapToResponse(
        Assignment assignment
    )
    {
        return new AssignmentResponse
        {
            Id = assignment.Id,
            Code = assignment.Code,
            Title = assignment.Title,
            Description = assignment.Description,
            Deadline = assignment.Deadline,
            MaximumMarks = assignment.MaximumMarks,
            IsPublished = assignment.IsPublished,
            AllowSubmissionUpdate =
                assignment.AllowSubmissionUpdate,
            TeacherAssignmentId =
                assignment.TeacherAssignmentId,

            TeacherId =
                assignment.TeacherAssignment.TeacherId,

            TeacherName =
                $"{assignment.TeacherAssignment.Teacher.FirstName} " +
                $"{assignment.TeacherAssignment.Teacher.LastName}",

            CourseId =
                assignment.TeacherAssignment.CourseId,

            CourseCode =
                assignment.TeacherAssignment.Course.Code,

            CourseName =
                assignment.TeacherAssignment.Course.Name,

            SubjectId =
                assignment.TeacherAssignment.SubjectId,

            SubjectCode =
                assignment.TeacherAssignment.Subject.Code,

            SubjectName =
                assignment.TeacherAssignment.Subject.Name,

            CreatedAt = assignment.CreatedAt,
            UpdatedAt = assignment.UpdatedAt
        };
    }
}