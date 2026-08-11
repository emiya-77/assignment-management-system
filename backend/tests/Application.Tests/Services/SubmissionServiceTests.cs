// CreateAsync
// - assignment must exist
// - unpublished assignment rejected
// - expired deadline rejected
// - unenrolled student rejected
// - duplicate submission rejected

// UpdateAsync
// - another student cannot update submission
// - updates disabled rejected
// - expired deadline rejected

// GradeAsync
// - another teacher cannot grade
// - marks above maximum rejected
// - valid grading sets Graded
// - GradedAt gets populated

// UpdateStatusAsync
// - another teacher cannot change status
// - invalid enum rejected

using Application.DTOs.Submissions;
using Application.Interfaces.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Application.Tests.Services;

public class SubmissionServiceTests
{
    private readonly Mock<ISubmissionRepository> _submissionRepository;
    private readonly Mock<IAssignmentRepository> _assignmentRepository;
    private readonly Mock<IEnrollmentRepository> _enrollmentRepository;
    private readonly SubmissionService _service;

    public SubmissionServiceTests()
    {
        _submissionRepository = new Mock<ISubmissionRepository>();
        _assignmentRepository = new Mock<IAssignmentRepository>();
        _enrollmentRepository = new Mock<IEnrollmentRepository>();

        _service = new SubmissionService(
            _submissionRepository.Object,
            _assignmentRepository.Object,
            _enrollmentRepository.Object
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenAssignmentIsNotPublished()
    {
        var assignment = CreateAssignment(
            isPublished: false
        );

        _assignmentRepository
            .Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(assignment);

        var request = new CreateSubmissionRequest
        {
            Answer = "My answer"
        };

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateAsync(1, 3, request)
            );

        Assert.Equal(
            "This assignment is not published.",
            exception.Message
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenDeadlineHasPassed()
    {
        var assignment = CreateAssignment(
            isPublished: true,
            deadline: DateTime.UtcNow.AddMinutes(-1)
        );

        _assignmentRepository
            .Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(assignment);

        var request = new CreateSubmissionRequest
        {
            Answer = "My answer"
        };

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateAsync(1, 3, request)
            );

        Assert.Equal(
            "The submission deadline has passed.",
            exception.Message
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenStudentIsNotEnrolled()
    {
        var assignment = CreateAssignment(
            isPublished: true
        );

        _assignmentRepository
            .Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(assignment);

        _enrollmentRepository
            .Setup(x => x.ExistsAsync(3, 1))
            .ReturnsAsync(false);

        var request = new CreateSubmissionRequest
        {
            Answer = "My answer"
        };

        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.CreateAsync(1, 3, request)
            );

        Assert.Equal(
            "You are not enrolled in the course for this assignment.",
            exception.Message
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenStudentAlreadySubmitted()
    {
        var assignment = CreateAssignment(
            isPublished: true
        );

        _assignmentRepository
            .Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(assignment);

        _enrollmentRepository
            .Setup(x => x.ExistsAsync(3, 1))
            .ReturnsAsync(true);

        _submissionRepository
            .Setup(x => x.ExistsAsync(1, 3))
            .ReturnsAsync(true);

        var request = new CreateSubmissionRequest
        {
            Answer = "My answer"
        };

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateAsync(1, 3, request)
            );

        Assert.Equal(
            "You have already submitted this assignment.",
            exception.Message
        );
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenStudentDoesNotOwnSubmission()
    {
        var submission = CreateSubmission(
            studentId: 99
        );

        _submissionRepository
            .Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(submission);

        var request = new UpdateSubmissionRequest
        {
            Answer = "Updated answer"
        };

        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.UpdateAsync(1, 3, request)
            );

        Assert.Equal(
            "You are not authorized to update this submission.",
            exception.Message
        );
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenUpdatesAreDisabled()
    {
        var submission = CreateSubmission(
            studentId: 3,
            allowSubmissionUpdate: false
        );

        _submissionRepository
            .Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(submission);

        var request = new UpdateSubmissionRequest
        {
            Answer = "Updated answer"
        };

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.UpdateAsync(1, 3, request)
            );

        Assert.Equal(
            "Submission updates are not allowed for this assignment.",
            exception.Message
        );
    }

    [Fact]
    public async Task GradeAsync_ShouldThrow_WhenTeacherDoesNotOwnAssignment()
    {
        var submission = CreateSubmission(
            teacherId: 99
        );

        _submissionRepository
            .Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(submission);

        var request = new GradeSubmissionRequest
        {
            Marks = 85,
            Feedback = "Good work."
        };

        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.GradeAsync(1, 2, request)
            );

        Assert.Equal(
            "You are not authorized to grade this submission.",
            exception.Message
        );
    }

    [Fact]
    public async Task GradeAsync_ShouldThrow_WhenMarksExceedMaximum()
    {
        var submission = CreateSubmission(
            teacherId: 2,
            maximumMarks: 100
        );

        _submissionRepository
            .Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(submission);

        var request = new GradeSubmissionRequest
        {
            Marks = 101,
            Feedback = "Too many marks."
        };

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.GradeAsync(1, 2, request)
            );

        Assert.Equal(
            "Marks cannot exceed the maximum marks of 100.",
            exception.Message
        );
    }

    [Fact]
    public async Task GradeAsync_ShouldSetGradedStatus()
    {
        var submission = CreateSubmission(
            teacherId: 2,
            maximumMarks: 100
        );

        _submissionRepository
            .Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(submission);

        _submissionRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Submission>()))
            .ReturnsAsync(submission);

        var request = new GradeSubmissionRequest
        {
            Marks = 85,
            Feedback = "Good work."
        };

        var result =
            await _service.GradeAsync(1, 2, request);

        Assert.NotNull(result);
        Assert.Equal(
            SubmissionStatus.Graded,
            submission.Status
        );
        Assert.Equal(85, submission.Marks);
        Assert.Equal(
            "Good work.",
            submission.Feedback
        );
        Assert.NotNull(submission.GradedAt);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrow_WhenStatusIsInvalid()
    {
        var submission = CreateSubmission(
            teacherId: 2
        );

        _submissionRepository
            .Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(submission);

        var request = new UpdateSubmissionStatusRequest
        {
            Status = (SubmissionStatus)999
        };

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.UpdateStatusAsync(1, 2, request)
            );

        Assert.Equal(
            "Invalid submission status.",
            exception.Message
        );
    }

    private static Assignment CreateAssignment(
        bool isPublished,
        DateTime? deadline = null,
        decimal maximumMarks = 100
    )
    {
        var teacherAssignment =
            new TeacherAssignment
            {
                Id = 1,
                TeacherId = 2,
                Teacher = new User
                {
                    Id = 2,
                    FirstName = "Kib",
                    LastName = "Teacher"
                },
                CourseId = 1,
                Course = new Course
                {
                    Id = 1,
                    Code = "CSE-101",
                    Name = "Computer Science"
                },
                SubjectId = 1,
                Subject = new Subject
                {
                    Id = 1,
                    Code = "DB-101",
                    Name = "Database Fundamentals"
                }
            };

        return new Assignment
        {
            Id = 1,
            Code = "DB-01",
            Title = "Database Assignment",
            Description = "Description",
            Deadline =
                deadline ?? DateTime.UtcNow.AddDays(7),
            MaximumMarks = maximumMarks,
            IsPublished = isPublished,
            AllowSubmissionUpdate = true,
            TeacherAssignmentId = 1,
            TeacherAssignment = teacherAssignment
        };
    }

    private static Submission CreateSubmission(
        int studentId = 3,
        int teacherId = 2,
        bool allowSubmissionUpdate = true,
        decimal maximumMarks = 100
    )
    {
        var assignment =
            CreateAssignment(
                isPublished: true,
                maximumMarks: maximumMarks
            );

        assignment.TeacherAssignment.TeacherId =
            teacherId;

        assignment.AllowSubmissionUpdate =
            allowSubmissionUpdate;

        return new Submission
        {
            Id = 1,
            AssignmentId = 1,
            Assignment = assignment,
            StudentId = studentId,
            Student = new User
            {
                Id = studentId,
                FirstName = "Ria",
                LastName = "Student",
                Email = "student@example.com"
            },
            Answer = "Original answer",
            Status = SubmissionStatus.Submitted,
            SubmittedAt = DateTime.UtcNow.AddMinutes(-10)
        };
    }
}