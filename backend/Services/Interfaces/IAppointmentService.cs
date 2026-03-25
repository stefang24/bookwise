using backend.DTOs;

namespace backend.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task MarkPastAppointmentsCompletedAsync();
        Task<ResultResponse<List<AvailableSlotResponse>>> GetAvailableSlotsAsync(int providerServiceId, string date);
        Task<ResultResponse<List<CalendarSlotResponse>>> GetCalendarSlotsAsync(int providerServiceId, string date);
        Task<ResultResponse<AppointmentResponse>> CreateAsync(int clientId, CreateAppointmentRequest request);
        Task<ResultResponse<bool>> CancelAsync(int userId, int appointmentId);
        Task<ResultResponse<List<AppointmentResponse>>> GetHistoryAsync(int userId, string role);
    }
}
