using backend.DTOs;

namespace backend.Services
{
    public interface IAdminService
    {
        Task<ResultResponse<List<AdminUserResponse>>> GetUsersAsync();
        Task<ResultResponse<bool>> SetUserActivationAsync(int adminUserId, int targetUserId, bool isActive);
        Task<ResultResponse<List<AdminAppointmentResponse>>> GetAppointmentsAsync();
        Task<ResultResponse<AdminStatsResponse>> GetStatsAsync(DateTime? fromUtc, DateTime? toUtc);
    }
}
