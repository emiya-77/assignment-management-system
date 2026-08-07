using Application.DTOs.Users;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;

    public UserService(
        IUserRepository userRepository,
        IPasswordService passwordService
    )
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }

    public async Task<List<UserResponse>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<UserResponse?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        return user is null
            ? null
            : MapToResponse(user);
    }

    public async Task<UserResponse> CreateAsync(
        CreateUserRequest request
    )
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailExists =
            await _userRepository.ExistsByEmailAsync(normalizedEmail);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "A user with this email already exists."
            );
        }

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = normalizedEmail,
            Role = request.Role,
            IsActive = true
        };

        user.PasswordHash = _passwordService.HashPassword(
            user,
            request.Password
        );

        var createdUser = await _userRepository.AddAsync(user);

        return MapToResponse(createdUser);
    }

    public async Task<UserResponse?> UpdateAsync(
        int id,
        UpdateUserRequest request
    )
    {
        var existingUser = await _userRepository.GetByIdAsync(id);

        if (existingUser is null)
        {
            return null;
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (!string.Equals(
                existingUser.Email,
                normalizedEmail,
                StringComparison.OrdinalIgnoreCase
            ))
        {
            var emailExists =
                await _userRepository.ExistsByEmailAsync(normalizedEmail);

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "A user with this email already exists."
                );
            }
        }

        existingUser.FirstName = request.FirstName.Trim();
        existingUser.LastName = request.LastName.Trim();
        existingUser.Email = normalizedEmail;
        existingUser.Role = request.Role;

        var updatedUser =
            await _userRepository.UpdateAsync(existingUser);

        return updatedUser is null
            ? null
            : MapToResponse(updatedUser);
    }

    public async Task<bool> UpdateStatusAsync(
        int id,
        bool isActive
    )
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
        {
            return false;
        }

        user.IsActive = isActive;

        await _userRepository.UpdateAsync(user);

        return true;
    }

    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}