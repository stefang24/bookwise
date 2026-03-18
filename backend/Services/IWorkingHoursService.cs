using backend.DTOs;

namespace backend.Services
{
    public interface IWorkingHoursService
    {
        Task<ResultResponse<List<ProviderWorkingHourDto>>> GetByProviderAsync(int providerId);
        Task<ResultResponse<List<ProviderWorkingHourDto>>> UpsertAsync(int providerId, UpdateWorkingHoursRequest request);
    }
}
