using Application.DTOs.Auth;

namespace Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}