using Application.DTOs.TeacherAssignments;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class TeacherAssignmentService
    : ITeacherAssignmentService
{
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly ITeacherAssignmentRepository
        _teacherAssignmentRepository;

    public TeacherAssignmentService(
        IUserRepository userRepository,
        ICourseRepository courseRepository,
        ISubjectRepository subjectRepository,
        ITeacherAssignmentRepository teacherAssignmentRepository
    )
    {
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _subjectRepository = subjectRepository;
        _teacherAssignmentRepository =
            teacherAssignmentRepository;
    }

    public async Task<List<TeacherAssignmentResponse>> GetAllAsync()
    {
        var teacherAssignments =
            await _teacherAssignmentRepository.GetAllAsync();

        return teacherAssignments
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<TeacherAssignmentResponse?> GetByIdAsync(int id)
    {
        var teacherAssignment =
            await _teacherAssignmentRepository.GetByIdAsync(id);

        return teacherAssignment is null
            ? null
            : MapToResponse(teacherAssignment);
    }

    public async Task<TeacherAssignmentResponse> CreateAsync(
        CreateTeacherAssignmentRequest request
    )
    {
        var teacher = await _userRepository.GetByIdAsync(
            request.TeacherId
        );

        if (teacher is null)
        {
            throw new InvalidOperationException(
                "Teacher not found."
            );
        }

        if (teacher.Role != UserRole.Teacher)
        {
            throw new InvalidOperationException(
                "The selected user is not a teacher."
            );
        }

        var course = await _courseRepository.GetByIdAsync(
            request.CourseId
        );

        if (course is null)
        {
            throw new InvalidOperationException(
                "Course not found."
            );
        }

        var subject = await _subjectRepository.GetByIdAsync(
            request.SubjectId
        );

        if (subject is null)
        {
            throw new InvalidOperationException(
                "Subject not found."
            );
        }

        var exists =
            await _teacherAssignmentRepository.ExistsAsync(
                request.TeacherId,
                request.CourseId,
                request.SubjectId
            );

        if (exists)
        {
            throw new InvalidOperationException(
                "This teacher is already assigned to this course and subject."
            );
        }

        var teacherAssignment = new TeacherAssignment
        {
            TeacherId = request.TeacherId,
            CourseId = request.CourseId,
            SubjectId = request.SubjectId
        };

        var createdTeacherAssignment =
            await _teacherAssignmentRepository.AddAsync(
                teacherAssignment
            );

        var result =
            await _teacherAssignmentRepository.GetByIdAsync(
                createdTeacherAssignment.Id
            );

        return MapToResponse(result!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _teacherAssignmentRepository.DeleteAsync(id);
    }

    private static TeacherAssignmentResponse MapToResponse(
        TeacherAssignment teacherAssignment
    )
    {
        return new TeacherAssignmentResponse
        {
            Id = teacherAssignment.Id,

            TeacherId = teacherAssignment.TeacherId,
            TeacherName =
                $"{teacherAssignment.Teacher.FirstName} " +
                $"{teacherAssignment.Teacher.LastName}",

            CourseId = teacherAssignment.CourseId,
            CourseCode = teacherAssignment.Course.Code,
            CourseName = teacherAssignment.Course.Name,

            SubjectId = teacherAssignment.SubjectId,
            SubjectCode = teacherAssignment.Subject.Code,
            SubjectName = teacherAssignment.Subject.Name,

            AssignedAt = teacherAssignment.AssignedAt
        };
    }
}