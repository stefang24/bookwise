using System.Security.Claims;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/provider-services")]
    public class ProviderServicesController : ControllerBase
    {
        private readonly IProviderCatalogService _providerCatalogService;

        public ProviderServicesController(IProviderCatalogService providerCatalogService)
        {
            _providerCatalogService = providerCatalogService;
        }

        [Authorize(Roles = "Provider")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProviderServiceRequest request)
        {
            int providerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ResultResponse<ProviderServiceResponse> response = await _providerCatalogService.CreateAsync(providerId, request);
            return Ok(response);
        }

        [Authorize(Roles = "Provider")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProviderServiceRequest request)
        {
            int providerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ResultResponse<ProviderServiceResponse> response = await _providerCatalogService.UpdateAsync(providerId, id, request);
            return Ok(response);
        }

        [Authorize(Roles = "Provider")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            int providerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ResultResponse<bool> response = await _providerCatalogService.DeleteAsync(providerId, id);
            return Ok(response);
        }

        [Authorize(Roles = "Provider")]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            int providerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ResultResponse<string> response = await _providerCatalogService.UploadImageAsync(providerId, file);
            return Ok(response);
        }

        [Authorize(Roles = "Provider")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMy()
        {
            int providerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ResultResponse<List<ProviderServiceResponse>> response = await _providerCatalogService.GetMyServicesAsync(providerId);
            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? category, [FromQuery] string? query)
        {
            ResultResponse<List<ProviderServiceResponse>> response = await _providerCatalogService.SearchAsync(category, query);
            return Ok(response);
        }

        [HttpGet("provider/{providerId:int}")]
        public async Task<IActionResult> GetByProvider(int providerId)
        {
            ResultResponse<List<ProviderServiceResponse>> response = await _providerCatalogService.GetByProviderAsync(providerId);
            return Ok(response);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> Categories()
        {
            ResultResponse<List<string>> response = await _providerCatalogService.GetCategoriesAsync();
            return Ok(response);
        }

        [HttpGet("providers")]
        public async Task<IActionResult> Providers([FromQuery] string? category, [FromQuery] string? city, [FromQuery] string? query, [FromQuery] string? sortBy)
        {
            ResultResponse<List<ProviderDirectoryItemResponse>> response = await _providerCatalogService.GetProvidersAsync(category, city, query, sortBy);
            return Ok(response);
        }
    }
}
