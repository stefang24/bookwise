using System.Security.Claims;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            ResultResponse<List<AdminUserResponse>> response = await _adminService.GetUsersAsync();
            return Ok(response);
        }

        [HttpPut("users/{id:int}/activation")]
        public async Task<IActionResult> SetActivation(int id, [FromBody] ToggleUserActivationRequest request)
        {
            int adminUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ResultResponse<bool> response = await _adminService.SetUserActivationAsync(adminUserId, id, request.IsActive);
            return Ok(response);
        }

        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointments()
        {
            ResultResponse<List<AdminAppointmentResponse>> response = await _adminService.GetAppointmentsAsync();
            return Ok(response);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats([FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc)
        {
            ResultResponse<AdminStatsResponse> response = await _adminService.GetStatsAsync(fromUtc, toUtc);
            return Ok(response);
        }
    }
}
