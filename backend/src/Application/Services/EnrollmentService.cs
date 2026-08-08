using Application.DTOs.Enrollments;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository
    )
    {
        _enrollmentRepository = enrollmentRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<List<EnrollmentResponse>> GetAllAsync()
    {
        var enrollments =
            await _enrollmentRepository.GetAllAsync();

        return enrollments
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<EnrollmentResponse?> GetByIdsAsync(
        int studentId,
        int courseId
    )
    {
        var enrollment =
            await _enrollmentRepository.GetByIdsAsync(
                studentId,
                courseId
            );

        return enrollment is null
            ? null
            : MapToResponse(enrollment);
    }

    public async Task<EnrollmentResponse> CreateAsync(
        CreateEnrollmentRequest request
    )
    {
        var student =
            await _userRepository.GetByIdAsync(
                request.StudentId
            );

        if (student is null)
        {
            throw new KeyNotFoundException(
                "Student not found."
            );
        }

        if (student.Role != UserRole.Student)
        {
            throw new InvalidOperationException(
                "The selected user is not a student."
            );
        }

        var course =
            await _courseRepository.GetByIdAsync(
                request.CourseId
            );

        if (course is null)
        {
            throw new KeyNotFoundException(
                "Course not found."
            );
        }

        var enrollmentExists =
            await _enrollmentRepository.ExistsAsync(
                request.StudentId,
                request.CourseId
            );

        if (enrollmentExists)
        {
            throw new InvalidOperationException(
                "The student is already enrolled in this course."
            );
        }

        var enrollment = new StudentCourse
        {
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrolledAt = DateTime.UtcNow
        };

        var createdEnrollment =
            await _enrollmentRepository.AddAsync(
                enrollment
            );

        // Load the navigation properties needed for the response.
        var enrollmentWithDetails =
            await _enrollmentRepository.GetByIdsAsync(
                createdEnrollment.StudentId,
                createdEnrollment.CourseId
            );

        return MapToResponse(enrollmentWithDetails!);
    }

    public async Task<bool> DeleteAsync(
        int studentId,
        int courseId
    )
    {
        return await _enrollmentRepository.DeleteAsync(
            studentId,
            courseId
        );
    }

    private static EnrollmentResponse MapToResponse(
        StudentCourse enrollment
    )
    {
        return new EnrollmentResponse
        {
            StudentId = enrollment.StudentId,

            StudentName =
                $"{enrollment.Student.FirstName} " +
                $"{enrollment.Student.LastName}",

            CourseId = enrollment.CourseId,
            CourseCode = enrollment.Course.Code,
            CourseName = enrollment.Course.Name,
            EnrolledAt = enrollment.EnrolledAt
        };
    }
}