using backend.Models;

namespace backend.Repositories
{
    public interface IWorkingHoursRepository
    {
        Task<List<ProviderWorkingHour>> GetByProviderAsync(int providerId);
        Task<ProviderWorkingHour?> GetByProviderAndDayAsync(int providerId, int dayOfWeek);
        Task AddAsync(ProviderWorkingHour entity);
        Task SaveChangesAsync();
    }
}
