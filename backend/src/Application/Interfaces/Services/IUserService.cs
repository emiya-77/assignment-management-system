using Application.DTOs.Users;

namespace Application.Interfaces.Services;

public interface IUserService
{
    Task<List<UserResponse>> GetAllAsync();
    Task<UserResponse?> GetByIdAsync(int id);
    Task<UserResponse> CreateAsync(CreateUserRequest request);
    Task<UserResponse?> UpdateAsync(
        int id,
        UpdateUserRequest request
    );
    Task<bool> UpdateStatusAsync(int id, bool isActive);
}