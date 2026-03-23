using backend.DTOs;
using backend.Models;
using backend.Repositories;
using backend.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace backend.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IProviderServiceRepository _providerServiceRepository;
        private readonly IWorkingHoursRepository _workingHoursRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IHubContext<ChatHub> _hubContext;

        public AppointmentService(IAppointmentRepository appointmentRepository, IProviderServiceRepository providerServiceRepository, IWorkingHoursRepository workingHoursRepository, IUserRepository userRepository, IChatRepository chatRepository, IHubContext<ChatHub> hubContext)
        {
            _appointmentRepository = appointmentRepository;
            _providerServiceRepository = providerServiceRepository;
            _workingHoursRepository = workingHoursRepository;
            _userRepository = userRepository;
            _chatRepository = chatRepository;
            _hubContext = hubContext;
        }

        public async Task MarkPastAppointmentsCompletedAsync()
        {
            List<Appointment> pastScheduled = await _appointmentRepository.GetPastScheduledAsync();

            if (pastScheduled.Count == 0)
                return;

            foreach (Appointment appointment in pastScheduled)
            {
                appointment.Status = AppointmentStatus.Completed;
            }

            await _appointmentRepository.SaveChangesAsync();
        }

        public async Task<ResultResponse<List<AvailableSlotResponse>>> GetAvailableSlotsAsync(int providerServiceId, string date)
        {
            await MarkPastAppointmentsCompletedAsync();

            ProviderService? service = await _providerServiceRepository.GetActiveByIdAsync(providerServiceId);

            if (service == null)
                return ResultResponse<List<AvailableSlotResponse>>.Fail("Service not found.");

            if (!DateOnly.TryParse(date, out DateOnly day))
                return ResultResponse<List<AvailableSlotResponse>>.Fail("Invalid date format. Use yyyy-MM-dd.");

            int dayOfWeek = (int)day.DayOfWeek;

            ProviderWorkingHour? workingHour = await _workingHoursRepository.GetByProviderAndDayAsync(service.ProviderId, dayOfWeek);

            if (workingHour == null || !workingHour.IsWorking || !workingHour.StartTime.HasValue || !workingHour.EndTime.HasValue)
                return ResultResponse<List<AvailableSlotResponse>>.Ok([]);

                // Treat working hours as UTC for consistency
                DateTime dayStartUtc = DateTime.SpecifyKind(day.ToDateTime(TimeOnly.FromTimeSpan(workingHour.StartTime.Value)), DateTimeKind.Utc);
                DateTime dayEndUtc = DateTime.SpecifyKind(day.ToDateTime(TimeOnly.FromTimeSpan(workingHour.EndTime.Value)), DateTimeKind.Utc);

                List<Appointment> booked = await _appointmentRepository.GetScheduledByProviderForDateAsync(service.ProviderId, day);

            List<AvailableSlotResponse> slots = [];
            DateTime nowUtc = DateTime.UtcNow;
                DateTime cursor = dayStartUtc;

                while (cursor.AddMinutes(service.DurationMinutes) <= dayEndUtc)
            {
                DateTime slotEnd = cursor.AddMinutes(service.DurationMinutes);

                bool conflicts = booked.Any(x => x.StartUtc < slotEnd && x.EndUtc > cursor);

                if (!conflicts && cursor >= nowUtc)
                {
                    slots.Add(new AvailableSlotResponse
                    {
                        StartUtc = DateTime.SpecifyKind(cursor, DateTimeKind.Unspecified),
                        EndUtc = DateTime.SpecifyKind(slotEnd, DateTimeKind.Unspecified)
                    });
                }

                cursor = cursor.AddMinutes(service.DurationMinutes);
            }

            return ResultResponse<List<AvailableSlotResponse>>.Ok(slots);
        }

        public async Task<ResultResponse<List<CalendarSlotResponse>>> GetCalendarSlotsAsync(int providerServiceId, string date)
        {
            await MarkPastAppointmentsCompletedAsync();

            ProviderService? service = await _providerServiceRepository.GetActiveByIdAsync(providerServiceId);

            if (service == null)
                return ResultResponse<List<CalendarSlotResponse>>.Fail("Service not found.");

            if (!DateOnly.TryParse(date, out DateOnly day))
                return ResultResponse<List<CalendarSlotResponse>>.Fail("Invalid date format. Use yyyy-MM-dd.");

            int dayOfWeek = (int)day.DayOfWeek;

            ProviderWorkingHour? workingHour = await _workingHoursRepository.GetByProviderAndDayAsync(service.ProviderId, dayOfWeek);

            if (workingHour == null || !workingHour.IsWorking || !workingHour.StartTime.HasValue || !workingHour.EndTime.HasValue)
                return ResultResponse<List<CalendarSlotResponse>>.Ok([]);

                // Treat working hours as UTC for consistency
                DateTime dayStartUtc = DateTime.SpecifyKind(day.ToDateTime(TimeOnly.FromTimeSpan(workingHour.StartTime.Value)), DateTimeKind.Utc);
                DateTime dayEndUtc = DateTime.SpecifyKind(day.ToDateTime(TimeOnly.FromTimeSpan(workingHour.EndTime.Value)), DateTimeKind.Utc);

                List<Appointment> booked = await _appointmentRepository.GetScheduledByProviderForDateAsync(service.ProviderId, day);

            DateTime nowUtc = DateTime.UtcNow;
                DateTime cursor = dayStartUtc;
            List<CalendarSlotResponse> slots = [];

                while (cursor.AddMinutes(service.DurationMinutes) <= dayEndUtc)
            {
                DateTime slotEnd = cursor.AddMinutes(service.DurationMinutes);
                bool conflicts = booked.Any(x => x.StartUtc < slotEnd && x.EndUtc > cursor);
                bool available = !conflicts && cursor >= nowUtc;

                slots.Add(new CalendarSlotResponse
                {
                    StartUtc = DateTime.SpecifyKind(cursor, DateTimeKind.Unspecified),
                    EndUtc = DateTime.SpecifyKind(slotEnd, DateTimeKind.Unspecified),
                    IsAvailable = available
                });

                cursor = cursor.AddMinutes(service.DurationMinutes);
            }

            return ResultResponse<List<CalendarSlotResponse>>.Ok(slots);
        }

        public async Task<ResultResponse<AppointmentResponse>> CreateAsync(int clientId, CreateAppointmentRequest request)
        {
            await MarkPastAppointmentsCompletedAsync();

            if (request.StartUtc.Kind != DateTimeKind.Utc)
                request.StartUtc = DateTime.SpecifyKind(request.StartUtc, DateTimeKind.Utc);

            ProviderService? service = await _providerServiceRepository.GetActiveWithProviderByIdAsync(request.ProviderServiceId);

            if (service == null)
                return ResultResponse<AppointmentResponse>.Fail("Service not found.");

            if (request.StartUtc < DateTime.UtcNow)
                return ResultResponse<AppointmentResponse>.Fail("Cannot book appointment in the past.");

            if (service.ProviderId == clientId)
                return ResultResponse<AppointmentResponse>.Fail("You cannot book your own service.");

            DateTime endUtc = request.StartUtc.AddMinutes(service.DurationMinutes);

            int dayOfWeek = (int)request.StartUtc.DayOfWeek;
            ProviderWorkingHour? workingHour = await _workingHoursRepository.GetByProviderAndDayAsync(service.ProviderId, dayOfWeek);

            if (workingHour == null || !workingHour.IsWorking || !workingHour.StartTime.HasValue || !workingHour.EndTime.HasValue)
                return ResultResponse<AppointmentResponse>.Fail("Provider is not working at selected time.");

            TimeSpan slotStart = request.StartUtc.TimeOfDay;
            TimeSpan slotEnd = endUtc.TimeOfDay;

            if (slotStart < workingHour.StartTime.Value || slotEnd > workingHour.EndTime.Value)
                return ResultResponse<AppointmentResponse>.Fail("Selected time is outside provider working hours.");

            bool hasConflict = await _appointmentRepository.HasConflictAsync(service.ProviderId, request.StartUtc, endUtc);

            if (hasConflict)
                return ResultResponse<AppointmentResponse>.Fail("Selected slot is no longer available.");

            Appointment appointment = new()
            {
                ProviderServiceId = request.ProviderServiceId,
                ClientId = clientId,
                StartUtc = request.StartUtc,
                EndUtc = endUtc
            };

            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            User? client = await _userRepository.GetByIdAsync(clientId);
            if (client == null)
                return ResultResponse<AppointmentResponse>.Fail("Client not found.");

            // Create notification for provider
            string clientName = client.CompanyName ?? (client.FirstName + " " + client.LastName).Trim();
            AppNotification notification = await _chatRepository.CreateAppointmentNotificationAsync(service.ProviderId, clientId, clientName, appointment);
            await _chatRepository.SaveChangesAsync();

            // Send notification via SignalR to provider
            AppNotificationResponse notificationResponse = new()
            {
                Id = notification.Id,
                Type = notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                RelatedUserId = notification.RelatedUserId,
                RelatedUserName = clientName,
                CreatedAtUtc = notification.CreatedAtUtc
            };
            await _hubContext.Clients.Group($"user-{service.ProviderId}").SendAsync("NotificationReceived", notificationResponse);

            return ResultResponse<AppointmentResponse>.Ok(Map(appointment, service, client));
        }

        public async Task<ResultResponse<bool>> CancelAsync(int userId, int appointmentId)
        {
            Appointment? appointment = await _appointmentRepository.GetByIdWithServiceAsync(appointmentId);

            if (appointment == null)
                return ResultResponse<bool>.Fail("Appointment not found.");

            bool isOwner = appointment.ClientId == userId || appointment.ProviderService.ProviderId == userId;
            if (!isOwner)
                return ResultResponse<bool>.Fail("You are not allowed to cancel this appointment.");

            if (appointment.Status != AppointmentStatus.Scheduled)
                return ResultResponse<bool>.Fail("Only scheduled appointments can be cancelled.");

            appointment.Status = AppointmentStatus.Cancelled;
            await _appointmentRepository.SaveChangesAsync();

            return ResultResponse<bool>.Ok(true);
        }

        public async Task<ResultResponse<List<AppointmentResponse>>> GetHistoryAsync(int userId, string role)
        {
            await MarkPastAppointmentsCompletedAsync();

            bool isProvider = role == UserRole.Provider.ToString();
            List<Appointment> appointments = await _appointmentRepository.GetHistoryAsync(userId, isProvider);

            List<AppointmentResponse> items = appointments.Select(x => new AppointmentResponse
            {
                Id = x.Id,
                ProviderServiceId = x.ProviderServiceId,
                ServiceName = x.ProviderService.Name,
                ProviderName = x.ProviderService.Provider.CompanyName ?? (x.ProviderService.Provider.FirstName + " " + x.ProviderService.Provider.LastName).Trim(),
                ProviderId = x.ProviderService.ProviderId,
                ProviderImagePath = x.ProviderService.Provider.ProfileImagePath,
                ClientName = (x.Client.FirstName + " " + x.Client.LastName).Trim(),
                ClientId = x.ClientId,
                ClientImagePath = x.Client.ProfileImagePath,
                StartUtc = x.StartUtc,
                EndUtc = x.EndUtc,
                Status = x.Status.ToString(),
                PriceEur = x.ProviderService.PriceEur,
                DurationMinutes = x.ProviderService.DurationMinutes
            }).ToList();

            return ResultResponse<List<AppointmentResponse>>.Ok(items);
        }

        private static AppointmentResponse Map(Appointment appointment, ProviderService service, User client)
        {
            return new AppointmentResponse
            {
                Id = appointment.Id,
                ProviderServiceId = appointment.ProviderServiceId,
                ServiceName = service.Name,
                ProviderName = service.Provider.CompanyName ?? (service.Provider.FirstName + " " + service.Provider.LastName).Trim(),
                ProviderId = service.ProviderId,
                ProviderImagePath = service.Provider.ProfileImagePath,
                ClientName = (client.FirstName + " " + client.LastName).Trim(),
                ClientId = client.Id,
                ClientImagePath = client.ProfileImagePath,
                StartUtc = appointment.StartUtc,
                EndUtc = appointment.EndUtc,
                Status = appointment.Status.ToString(),
                PriceEur = service.PriceEur,
                DurationMinutes = service.DurationMinutes
            };
        }
    }
}
