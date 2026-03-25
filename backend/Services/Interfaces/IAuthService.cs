using backend.DTOs;

namespace backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResultResponse<AuthResponse>> LoginAsync(LoginRequest request);
        Task<ResultResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    }
}
