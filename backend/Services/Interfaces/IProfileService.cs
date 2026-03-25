using backend.DTOs;

namespace backend.Services.Interfaces
{
    public interface IProfileService
    {
        Task<ResultResponse<ProfileResponse>> GetProfileAsync(int userId);
        Task<ResultResponse<ProfileResponse>> UpdateProfileAsync(int userId, UpdateProfileRequest request);
        Task<ResultResponse<ProfileResponse>> UploadProfileImageAsync(int userId, IFormFile file);
    }
}
