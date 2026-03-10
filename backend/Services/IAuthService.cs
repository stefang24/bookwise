using backend.DTOs;

namespace backend.Services
{
    public interface IAuthService
    {
        Task<ResultResponse<AuthResponse>> LoginAsync(LoginRequest request);
        Task<ResultResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    }
}
