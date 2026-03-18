using backend.DTOs;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class ProviderCatalogService : IProviderCatalogService
    {
        private readonly IProviderServiceRepository _providerServiceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _environment;
        private static readonly HashSet<string> AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
        private static readonly List<string> PredefinedCategories =
        [
            "Haircut",
            "Barber",
            "Nails",
            "Makeup",
            "Skincare",
            "Massage",
            "Eyebrows",
            "Eyelashes",
            "Waxing",
            "Spa",
            "Fitness",
            "Personal Training",
            "Physiotherapy",
            "Photography",
            "Tutoring",
            "Cleaning",
            "Repair",
            "Plumbing",
            "Electrical",
            "Car Service"
        ];
        private const long MaxFileSize = 5 * 1024 * 1024;
        private const string DefaultServiceImageUrl = "/images/services/default-service.svg";

        public ProviderCatalogService(IProviderServiceRepository providerServiceRepository, IUserRepository userRepository, IWebHostEnvironment environment)
        {
            _providerServiceRepository = providerServiceRepository;
            _userRepository = userRepository;
            _environment = environment;
        }

        public async Task<ResultResponse<ProviderServiceResponse>> CreateAsync(int providerId, ProviderServiceRequest request)
        {
            User? provider = await _userRepository.GetByIdAsync(providerId);
            if (provider == null || provider.Role != UserRole.Provider)
                return ResultResponse<ProviderServiceResponse>.Fail("Provider not found.");

            if (request.DurationMinutes < 5)
                return ResultResponse<ProviderServiceResponse>.Fail("Duration must be at least 5 minutes.");

            if (request.PriceEur < 0)
                return ResultResponse<ProviderServiceResponse>.Fail("Price must be non-negative.");

            ProviderService entity = new()
            {
                ProviderId = providerId,
                Name = request.Name.Trim(),
                Category = request.Category.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? DefaultServiceImageUrl : request.ImageUrl.Trim(),
                PriceEur = request.PriceEur,
                DurationMinutes = request.DurationMinutes
            };

            await _providerServiceRepository.AddAsync(entity);

            return ResultResponse<ProviderServiceResponse>.Ok(Map(entity, provider));
        }

        public async Task<ResultResponse<ProviderServiceResponse>> UpdateAsync(int providerId, int serviceId, ProviderServiceRequest request)
        {
            ProviderService? service = await _providerServiceRepository.GetByIdAndProviderWithProviderAsync(serviceId, providerId);

            if (service == null)
                return ResultResponse<ProviderServiceResponse>.Fail("Service not found.");

            if (request.DurationMinutes < 5)
                return ResultResponse<ProviderServiceResponse>.Fail("Duration must be at least 5 minutes.");

            if (request.PriceEur < 0)
                return ResultResponse<ProviderServiceResponse>.Fail("Price must be non-negative.");

            string newImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? DefaultServiceImageUrl : request.ImageUrl.Trim();

            if (service.ImageUrl != newImageUrl && service.ImageUrl != DefaultServiceImageUrl && !string.IsNullOrEmpty(service.ImageUrl))
                DeleteImageFile(service.ImageUrl);

            service.Name = request.Name.Trim();
            service.Category = request.Category.Trim();
            service.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
            service.ImageUrl = newImageUrl;
            service.PriceEur = request.PriceEur;
            service.DurationMinutes = request.DurationMinutes;

            await _providerServiceRepository.SaveChangesAsync();

            return ResultResponse<ProviderServiceResponse>.Ok(Map(service, service.Provider));
        }

        public async Task<ResultResponse<bool>> DeleteAsync(int providerId, int serviceId)
        {
            ProviderService? service = await _providerServiceRepository.GetByIdAndProviderAsync(serviceId, providerId);

            if (service == null)
                return ResultResponse<bool>.Fail("Service not found.");

            if (!string.IsNullOrEmpty(service.ImageUrl) && service.ImageUrl != DefaultServiceImageUrl)
                DeleteImageFile(service.ImageUrl);

            service.IsActive = false;
            await _providerServiceRepository.SaveChangesAsync();

            return ResultResponse<bool>.Ok(true);
        }

        public async Task<ResultResponse<string>> UploadImageAsync(int providerId, IFormFile file)
        {
            User? provider = await _userRepository.GetByIdAsync(providerId);
            if (provider == null || provider.Role != UserRole.Provider)
                return ResultResponse<string>.Fail("Provider not found.");

            if (file.Length == 0)
                return ResultResponse<string>.Fail("File is empty.");

            if (file.Length > MaxFileSize)
                return ResultResponse<string>.Fail("File size must be less than 5MB.");

            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
                return ResultResponse<string>.Fail("Only image files are allowed (.jpg, .jpeg, .png, .gif, .webp).");

            if (!file.ContentType.StartsWith("image/"))
                return ResultResponse<string>.Fail("Only image files are allowed.");

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "services");
            Directory.CreateDirectory(uploadsFolder);

            string fileName = $"svc_{providerId}_{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (FileStream stream = new(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string imageUrl = $"/images/services/{fileName}";
            return ResultResponse<string>.Ok(imageUrl);
        }

        private void DeleteImageFile(string imageUrl)
        {
            string fileName = Path.GetFileName(imageUrl);
            string filePath = Path.Combine(_environment.WebRootPath, "images", "services", fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public async Task<ResultResponse<List<ProviderServiceResponse>>> GetMyServicesAsync(int providerId)
        {
            List<ProviderService> services = await _providerServiceRepository.GetByProviderActiveAsync(providerId);

            return ResultResponse<List<ProviderServiceResponse>>.Ok(services.Select(x => Map(x, x.Provider)).ToList());
        }

        public async Task<ResultResponse<List<ProviderServiceResponse>>> SearchAsync(string? category, string? query)
        {
            List<ProviderService> services = await _providerServiceRepository.SearchActiveAsync(category, query);

            return ResultResponse<List<ProviderServiceResponse>>.Ok(services.Select(x => Map(x, x.Provider)).ToList());
        }

        public async Task<ResultResponse<List<ProviderServiceResponse>>> GetByProviderAsync(int providerId)
        {
            List<ProviderService> services = await _providerServiceRepository.GetByProviderActiveAsync(providerId);

            return ResultResponse<List<ProviderServiceResponse>>.Ok(services.Select(x => Map(x, x.Provider)).ToList());
        }

        public async Task<ResultResponse<List<string>>> GetCategoriesAsync()
        {
            List<string> customCategories = await _providerServiceRepository.GetActiveCategoriesAsync();

            List<string> categories = PredefinedCategories
                .Concat(customCategories)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();

            return ResultResponse<List<string>>.Ok(categories);
        }

        public async Task<ResultResponse<List<ProviderDirectoryItemResponse>>> GetProvidersAsync(string? category, string? city, string? query, string? sortBy)
        {
            List<User> providers = await _userRepository.GetProvidersFilteredAsync(category, city, query);

            List<ProviderDirectoryItemResponse> result = providers
                .Select(x => new ProviderDirectoryItemResponse
                {
                    Id = x.Id,
                    Name = x.CompanyName ?? (x.FirstName + " " + x.LastName).Trim(),
                    Username = x.Username,
                    ProfileImagePath = x.ProfileImagePath,
                    PrimaryCategory = x.PrimaryCategory,
                    City = x.City,
                    Address = x.Address,
                    ServicesCount = x.ProviderServices.Count(s => s.IsActive)
                })
                .ToList();

            string normalizedSort = (sortBy ?? "name").Trim().ToLower();

            result = normalizedSort switch
            {
                "services-desc" => result.OrderByDescending(x => x.ServicesCount).ThenBy(x => x.Name).ToList(),
                "city" => result.OrderBy(x => x.City).ThenBy(x => x.Name).ToList(),
                _ => result.OrderBy(x => x.Name).ToList()
            };

            return ResultResponse<List<ProviderDirectoryItemResponse>>.Ok(result);
        }

        private static ProviderServiceResponse Map(ProviderService service, User provider)
        {
            return new ProviderServiceResponse
            {
                Id = service.Id,
                ProviderId = service.ProviderId,
                ProviderName = provider.CompanyName ?? (provider.FirstName + " " + provider.LastName).Trim(),
                ProviderImagePath = provider.ProfileImagePath,
                ProviderCity = provider.City,
                Name = service.Name,
                Category = service.Category,
                Description = service.Description,
                ImageUrl = service.ImageUrl,
                PriceEur = service.PriceEur,
                DurationMinutes = service.DurationMinutes,
                IsActive = service.IsActive
            };
        }
    }
}
