// CreateAsync
// - valid assignment succeeds
// - duplicate assignment code rejected
// - nonexistent TeacherAssignment rejected
// - teacher cannot create for another teacher

// UpdateAsync
// - teacher cannot update another teacher's assignment

// DeleteAsync
// - teacher cannot delete another teacher's assignment

// PublishAsync
// - teacher cannot publish another teacher's assignment

using Application.DTOs.Assignments;
using Application.Interfaces.Repositories;
using Application.Services;
using Domain.Entities;
using Moq;

namespace Application.Tests.Services;

public class AssignmentServiceTests
{
    private readonly Mock<IAssignmentRepository> _assignmentRepository;
    private readonly Mock<ITeacherAssignmentRepository> _teacherAssignmentRepository;
    private readonly AssignmentService _service;

    public AssignmentServiceTests()
    {
        _assignmentRepository = new Mock<IAssignmentRepository>();
        _teacherAssignmentRepository =
            new Mock<ITeacherAssignmentRepository>();

        _service = new AssignmentService(
            _assignmentRepository.Object,
            _teacherAssignmentRepository.Object
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAssignment_WhenRequestIsValid()
    {
        var teacherId = 2;

        var teacherAssignment = CreateTeacherAssignment(
            id: 1,
            teacherId: teacherId
        );

        var request = new CreateAssignmentRequest
        {
            Code = " db-01 ",
            Title = "Database Assignment",
            Description = "Explain primary keys.",
            Deadline = DateTime.UtcNow.AddDays(7),
            MaximumMarks = 100,
            AllowSubmissionUpdate = true,
            TeacherAssignmentId = 1
        };

        var createdAssignment = new Assignment
        {
            Id = 1,
            Code = "DB-01",
            Title = "Database Assignment",
            Description = "Explain primary keys.",
            Deadline = request.Deadline,
            MaximumMarks = 100,
            AllowSubmissionUpdate = true,
            IsPublished = false,
            TeacherAssignmentId = 1,
            TeacherAssignment = teacherAssignment
        };

        _assignmentRepository
            .Setup(x => x.ExistsByCodeAsync("DB-01"))
            .ReturnsAsync(false);

        _teacherAssignmentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(teacherAssignment);

        _assignmentRepository
            .Setup(x => x.AddAsync(It.IsAny<Assignment>()))
            .ReturnsAsync(createdAssignment);

        _assignmentRepository
            .Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(createdAssignment);

        var result = await _service.CreateAsync(
            teacherId,
            request
        );

        Assert.NotNull(result);
        Assert.Equal("DB-01", result.Code);
        Assert.False(result.IsPublished);
        Assert.Equal(100, result.MaximumMarks);

        _assignmentRepository.Verify(
            x => x.AddAsync(It.IsAny<Assignment>()),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenCodeAlreadyExists()
    {
        var request = new CreateAssignmentRequest
        {
            Code = " db-01 ",
            Title = "Database Assignment",
            Description = "Description",
            Deadline = DateTime.UtcNow.AddDays(7),
            MaximumMarks = 100,
            TeacherAssignmentId = 1
        };

        _assignmentRepository
            .Setup(x => x.ExistsByCodeAsync("DB-01"))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAsync(2, request)
        );

        Assert.Equal(
            "An assignment with this code already exists.",
            exception.Message
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenTeacherDoesNotOwnTeacherAssignment()
    {
        var teacherAssignment = CreateTeacherAssignment(
            id: 1,
            teacherId: 99
        );

        var request = new CreateAssignmentRequest
        {
            Code = "DB-01",
            Title = "Database Assignment",
            Description = "Description",
            Deadline = DateTime.UtcNow.AddDays(7),
            MaximumMarks = 100,
            TeacherAssignmentId = 1
        };

        _assignmentRepository
            .Setup(x => x.ExistsByCodeAsync("DB-01"))
            .ReturnsAsync(false);

        _teacherAssignmentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(teacherAssignment);

        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.CreateAsync(2, request)
            );

        Assert.Equal(
            "You are not authorized to create an assignment for this teacher assignment.",
            exception.Message
        );
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenTeacherDoesNotOwnAssignment()
    {
        var assignment = CreateAssignment(
            id: 1,
            teacherId: 99
        );

        var teacherAssignment = assignment.TeacherAssignment;

        _assignmentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(assignment);

        _teacherAssignmentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(teacherAssignment);

        var request = new UpdateAssignmentRequest
        {
            Code = "DB-01",
            Title = "Updated",
            Description = "Updated description",
            Deadline = DateTime.UtcNow.AddDays(5),
            MaximumMarks = 100,
            AllowSubmissionUpdate = true
        };

        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.UpdateAsync(
                    1,
                    2,
                    request
                )
            );

        Assert.Equal(
            "You are not authorized to update this assignment.",
            exception.Message
        );
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenTeacherDoesNotOwnAssignment()
    {
        var assignment = CreateAssignment(
            id: 1,
            teacherId: 99
        );

        _assignmentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(assignment);

        _teacherAssignmentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(assignment.TeacherAssignment);

        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.DeleteAsync(1, 2)
            );

        Assert.Equal(
            "You are not authorized to delete this assignment.",
            exception.Message
        );
    }

    [Fact]
    public async Task PublishAsync_ShouldThrow_WhenTeacherDoesNotOwnAssignment()
    {
        var assignment = CreateAssignment(
            id: 1,
            teacherId: 99
        );

        _assignmentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(assignment);

        _teacherAssignmentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(assignment.TeacherAssignment);

        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.PublishAsync(1, 2)
            );

        Assert.Equal(
            "You are not authorized to publish this assignment.",
            exception.Message
        );
    }

    private static Assignment CreateAssignment(
        int id,
        int teacherId
    )
    {
        var teacherAssignment =
            CreateTeacherAssignment(
                id: 1,
                teacherId: teacherId
            );

        return new Assignment
        {
            Id = id,
            Code = "DB-01",
            Title = "Database Assignment",
            Description = "Description",
            Deadline = DateTime.UtcNow.AddDays(7),
            MaximumMarks = 100,
            IsPublished = false,
            AllowSubmissionUpdate = true,
            TeacherAssignmentId = teacherAssignment.Id,
            TeacherAssignment = teacherAssignment
        };
    }

    private static TeacherAssignment CreateTeacherAssignment(
        int id,
        int teacherId
    )
    {
        return new TeacherAssignment
        {
            Id = id,
            TeacherId = teacherId,
            Teacher = new User
            {
                Id = teacherId,
                FirstName = "Test",
                LastName = "Teacher",
                Email = $"teacher{teacherId}@example.com"
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
    }
}