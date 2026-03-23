using backend.Models;

namespace backend.Repositories
{
    public interface IProviderServiceRepository
    {
        Task<ProviderService?> GetActiveByIdAsync(int id);
        Task<ProviderService?> GetActiveWithProviderByIdAsync(int id);
        Task<ProviderService?> GetByIdAndProviderAsync(int serviceId, int providerId);
        Task<ProviderService?> GetByIdAndProviderWithProviderAsync(int serviceId, int providerId);
        Task<List<ProviderService>> GetByProviderActiveAsync(int providerId);
        Task<List<ProviderService>> GetTopBookedActiveAsync(int limit);
        Task<List<ProviderService>> SearchActiveAsync(string? category, string? query);
        Task<List<string>> GetActiveCategoriesAsync();
        Task AddAsync(ProviderService entity);
        Task SaveChangesAsync();
    }
}
