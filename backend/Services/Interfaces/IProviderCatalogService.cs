using backend.DTOs;

namespace backend.Services.Interfaces
{
    public interface IProviderCatalogService
    {
        Task<ResultResponse<ProviderServiceResponse>> CreateAsync(int providerId, ProviderServiceRequest request);
        Task<ResultResponse<ProviderServiceResponse>> UpdateAsync(int providerId, int serviceId, ProviderServiceRequest request);
        Task<ResultResponse<bool>> DeleteAsync(int providerId, int serviceId);
        Task<ResultResponse<string>> UploadImageAsync(int providerId, IFormFile file);
        Task<ResultResponse<List<ProviderServiceResponse>>> GetMyServicesAsync(int providerId);
        Task<ResultResponse<List<ProviderServiceResponse>>> SearchAsync(string? category, string? query);
        Task<ResultResponse<List<ProviderServiceResponse>>> GetFeaturedServicesAsync(int limit);
        Task<ResultResponse<List<ProviderServiceResponse>>> GetByProviderAsync(int providerId);
        Task<ResultResponse<List<string>>> GetCategoriesAsync();
        Task<ResultResponse<List<ProviderDirectoryItemResponse>>> GetProvidersAsync(string? category, string? city, string? query, string? sortBy);
        Task<ResultResponse<List<ProviderDirectoryItemResponse>>> GetTopProvidersAsync(int limit);
    }
}
