using System.Security.Claims;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/working-hours")]
    public class WorkingHoursController : ControllerBase
    {
        private readonly IWorkingHoursService _workingHoursService;

        public WorkingHoursController(IWorkingHoursService workingHoursService)
        {
            _workingHoursService = workingHoursService;
        }

        [Authorize(Roles = "Provider")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMy()
        {
            int providerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ResultResponse<List<ProviderWorkingHourDto>> response = await _workingHoursService.GetByProviderAsync(providerId);
            return Ok(response);
        }

        [Authorize(Roles = "Provider")]
        [HttpPut("my")]
        public async Task<IActionResult> UpsertMy([FromBody] UpdateWorkingHoursRequest request)
        {
            int providerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ResultResponse<List<ProviderWorkingHourDto>> response = await _workingHoursService.UpsertAsync(providerId, request);
            return Ok(response);
        }

        [HttpGet("provider/{providerId:int}")]
        public async Task<IActionResult> GetByProvider(int providerId)
        {
            ResultResponse<List<ProviderWorkingHourDto>> response = await _workingHoursService.GetByProviderAsync(providerId);
            return Ok(response);
        }
    }
}
