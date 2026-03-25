using System.Security.Claims;
using backend.DTOs;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("available-slots")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] int providerServiceId, [FromQuery] string date)
        {
            ResultResponse<List<AvailableSlotResponse>> response = await _appointmentService.GetAvailableSlotsAsync(providerServiceId, date);
            return Ok(response);
        }

        [HttpGet("calendar-slots")]
        public async Task<IActionResult> GetCalendarSlots([FromQuery] int providerServiceId, [FromQuery] string date)
        {
            ResultResponse<List<CalendarSlotResponse>> response = await _appointmentService.GetCalendarSlotsAsync(providerServiceId, date);
            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ResultResponse<AppointmentResponse> response = await _appointmentService.CreateAsync(userId, request);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ResultResponse<bool> response = await _appointmentService.CancelAsync(userId, id);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("my-history")]
        public async Task<IActionResult> MyHistory()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            string role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            await _appointmentService.MarkPastAppointmentsCompletedAsync();
            ResultResponse<List<AppointmentResponse>> response = await _appointmentService.GetHistoryAsync(userId, role);
            return Ok(response);
        }
    }
}
