using Application.DTOs.Submissions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class SubmissionService : ISubmissionService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public SubmissionService(
        ISubmissionRepository submissionRepository,
        IAssignmentRepository assignmentRepository,
        IEnrollmentRepository enrollmentRepository
    )
    {
        _submissionRepository = submissionRepository;
        _assignmentRepository = assignmentRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<List<SubmissionResponse>> GetAllAsync()
    {
        var submissions =
            await _submissionRepository.GetAllAsync();

        return submissions
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<List<SubmissionResponse>> GetByStudentIdAsync(
        int studentId
    )
    {
        var submissions =
            await _submissionRepository
                .GetByStudentIdAsync(studentId);

        return submissions
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<SubmissionResponse?> GetByIdAsync(int id)
    {
        var submission =
            await _submissionRepository
                .GetByIdWithDetailsAsync(id);

        return submission is null
            ? null
            : MapToResponse(submission);
    }

    public async Task<List<SubmissionResponse>>
        GetByAssignmentIdAsync(
            int assignmentId,
            int teacherId
        )
    {
        var assignment =
            await _assignmentRepository
                .GetByIdWithDetailsAsync(assignmentId);

        if (assignment is null)
        {
            throw new KeyNotFoundException(
                "Assignment not found."
            );
        }

        if (
            assignment.TeacherAssignment.TeacherId
            != teacherId
        )
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to view submissions for this assignment."
            );
        }

        var submissions =
            await _submissionRepository
                .GetByAssignmentIdAsync(assignmentId);

        return submissions
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<SubmissionResponse> CreateAsync(
        int assignmentId,
        int studentId,
        CreateSubmissionRequest request
    )
    {
        var assignment =
            await _assignmentRepository
                .GetByIdWithDetailsAsync(assignmentId);

        if (assignment is null)
        {
            throw new KeyNotFoundException(
                "Assignment not found."
            );
        }

        if (!assignment.IsPublished)
        {
            throw new InvalidOperationException(
                "This assignment is not published."
            );
        }

        if (assignment.Deadline < DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "The submission deadline has passed."
            );
        }

        var courseId =
            assignment.TeacherAssignment.CourseId;

        var isEnrolled =
            await _enrollmentRepository.ExistsAsync(
                studentId,
                courseId
            );

        if (!isEnrolled)
        {
            throw new UnauthorizedAccessException(
                "You are not enrolled in the course for this assignment."
            );
        }

        var submissionExists =
            await _submissionRepository.ExistsAsync(
                assignmentId,
                studentId
            );

        if (submissionExists)
        {
            throw new InvalidOperationException(
                "You have already submitted this assignment."
            );
        }

        var submission = new Submission
        {
            AssignmentId = assignmentId,
            StudentId = studentId,
            Answer = request.Answer.Trim(),
            Status = SubmissionStatus.Submitted
        };

        var createdSubmission =
            await _submissionRepository.AddAsync(submission);

        var submissionWithDetails =
            await _submissionRepository
                .GetByIdWithDetailsAsync(
                    createdSubmission.Id
                );

        return MapToResponse(submissionWithDetails!);
    }

    public async Task<SubmissionResponse?> UpdateAsync(
        int id,
        int studentId,
        UpdateSubmissionRequest request
    )
    {
        var submission =
            await _submissionRepository
                .GetByIdWithDetailsAsync(id);

        if (submission is null)
        {
            return null;
        }

        if (submission.StudentId != studentId)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to update this submission."
            );
        }

        if (!submission.Assignment.AllowSubmissionUpdate)
        {
            throw new InvalidOperationException(
                "Submission updates are not allowed for this assignment."
            );
        }

        if (submission.Assignment.Deadline < DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "The submission deadline has passed."
            );
        }

        submission.Answer = request.Answer.Trim();
        submission.UpdatedAt = DateTime.UtcNow;

        await _submissionRepository.UpdateAsync(submission);

        var updatedSubmission =
            await _submissionRepository
                .GetByIdWithDetailsAsync(id);

        return updatedSubmission is null
            ? null
            : MapToResponse(updatedSubmission);
    }

    public async Task<SubmissionResponse?> GradeAsync(
        int id,
        int teacherId,
        GradeSubmissionRequest request
    )
    {
        var submission =
            await _submissionRepository
                .GetByIdWithDetailsAsync(id);

        if (submission is null)
        {
            return null;
        }

        var assignment = submission.Assignment;

        if (
            assignment.TeacherAssignment.TeacherId
            != teacherId
        )
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to grade this submission."
            );
        }

        if (request.Marks > assignment.MaximumMarks)
        {
            throw new InvalidOperationException(
                $"Marks cannot exceed the maximum marks of {assignment.MaximumMarks}."
            );
        }

        submission.Marks = request.Marks;

        submission.Feedback =
            string.IsNullOrWhiteSpace(request.Feedback)
                ? null
                : request.Feedback.Trim();

        submission.Status = SubmissionStatus.Graded;
        submission.GradedAt = DateTime.UtcNow;

        await _submissionRepository.UpdateAsync(submission);

        var gradedSubmission =
            await _submissionRepository
                .GetByIdWithDetailsAsync(id);

        return gradedSubmission is null
            ? null
            : MapToResponse(gradedSubmission);
    }

    public async Task<SubmissionResponse?> UpdateStatusAsync(
        int id,
        int teacherId,
        UpdateSubmissionStatusRequest request
    )
    {
        var submission =
            await _submissionRepository
                .GetByIdWithDetailsAsync(id);

        if (submission is null)
        {
            return null;
        }

        if (
            submission.Assignment.TeacherAssignment.TeacherId
            != teacherId
        )
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to update this submission status."
            );
        }

        submission.Status = request.Status;
        submission.UpdatedAt = DateTime.UtcNow;

        await _submissionRepository.UpdateAsync(submission);

        var updatedSubmission =
            await _submissionRepository
                .GetByIdWithDetailsAsync(id);

        return updatedSubmission is null
            ? null
            : MapToResponse(updatedSubmission);
    }

    private static SubmissionResponse MapToResponse(
        Submission submission
    )
    {
        return new SubmissionResponse
        {
            Id = submission.Id,

            AssignmentId = submission.AssignmentId,

            AssignmentCode =
                submission.Assignment.Code,

            AssignmentTitle =
                submission.Assignment.Title,
            
            TeacherId = submission.Assignment.TeacherAssignment.TeacherId,

            StudentId = submission.StudentId,

            StudentName =
                $"{submission.Student.FirstName} " +
                $"{submission.Student.LastName}",

            StudentEmail =
                submission.Student.Email,

            Answer = submission.Answer,

            Status = submission.Status,

            Marks = submission.Marks,

            Feedback = submission.Feedback,

            SubmittedAt = submission.SubmittedAt,

            UpdatedAt = submission.UpdatedAt,

            GradedAt = submission.GradedAt
        };
    }
}