using Application.DTOs.Subjects;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjectRepository;

    public SubjectService(
        ISubjectRepository subjectRepository
    )
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<List<SubjectResponse>> GetAllAsync()
    {
        var subjects = await _subjectRepository.GetAllAsync();

        return subjects
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<SubjectResponse?> GetByIdAsync(int id)
    {
        var subject = await _subjectRepository.GetByIdAsync(id);

        return subject is null
            ? null
            : MapToResponse(subject);
    }

    public async Task<SubjectResponse> CreateAsync(
        CreateSubjectRequest request
    )
    {
        var normalizedCode =
            request.Code.Trim().ToUpperInvariant();

        var codeExists =
            await _subjectRepository.ExistsByCodeAsync(
                normalizedCode
            );

        if (codeExists)
        {
            throw new InvalidOperationException(
                "A subject with this code already exists."
            );
        }

        var subject = new Subject
        {
            Code = normalizedCode,
            Name = request.Name.Trim(),
            Description =
                string.IsNullOrWhiteSpace(request.Description)
                    ? null
                    : request.Description.Trim()
        };

        var createdSubject =
            await _subjectRepository.AddAsync(subject);

        return MapToResponse(createdSubject);
    }

    public async Task<SubjectResponse?> UpdateAsync(
        int id,
        UpdateSubjectRequest request
    )
    {
        var existingSubject =
            await _subjectRepository.GetByIdAsync(id);

        if (existingSubject is null)
        {
            return null;
        }

        var normalizedCode =
            request.Code.Trim().ToUpperInvariant();

        if (!string.Equals(
                existingSubject.Code,
                normalizedCode,
                StringComparison.OrdinalIgnoreCase
            ))
        {
            var codeExists =
                await _subjectRepository.ExistsByCodeAsync(
                    normalizedCode
                );

            if (codeExists)
            {
                throw new InvalidOperationException(
                    "A subject with this code already exists."
                );
            }
        }

        existingSubject.Code = normalizedCode;
        existingSubject.Name = request.Name.Trim();
        existingSubject.Description =
            string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();

        var updatedSubject =
            await _subjectRepository.UpdateAsync(existingSubject);

        return updatedSubject is null
            ? null
            : MapToResponse(updatedSubject);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _subjectRepository.DeleteAsync(id);
    }

    private static SubjectResponse MapToResponse(
        Subject subject
    )
    {
        return new SubjectResponse
        {
            Id = subject.Id,
            Code = subject.Code,
            Name = subject.Name,
            Description = subject.Description,
            CreatedAt = subject.CreatedAt
        };
    }
}