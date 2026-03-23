using backend.DTOs;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<ResultResponse<List<AdminUserResponse>>> GetUsersAsync()
        {
            List<User> users = await _adminRepository.GetAllUsersAsync();

            List<AdminUserResponse> result = users.Select(x => new AdminUserResponse
            {
                Id = x.Id,
                Email = x.Email,
                Username = x.Username,
                Name = x.CompanyName ?? (x.FirstName + " " + x.LastName).Trim(),
                Role = x.Role.ToString(),
                IsActive = x.IsActive,
                City = x.City,
                CreatedAt = x.CreatedAt
            }).ToList();

            return ResultResponse<List<AdminUserResponse>>.Ok(result);
        }

        public async Task<ResultResponse<bool>> SetUserActivationAsync(int adminUserId, int targetUserId, bool isActive)
        {
            if (adminUserId == targetUserId)
                return ResultResponse<bool>.Fail("You cannot change your own activation status.");

            User? user = await _adminRepository.GetUserByIdAsync(targetUserId);
            if (user == null)
                return ResultResponse<bool>.Fail("User not found.");

            if (user.Role == UserRole.Admin)
                return ResultResponse<bool>.Fail("Admin account cannot be changed.");

            user.IsActive = isActive;
            await _adminRepository.SaveChangesAsync();

            return ResultResponse<bool>.Ok(true);
        }

        public async Task<ResultResponse<List<AdminAppointmentResponse>>> GetAppointmentsAsync()
        {
            List<Appointment> appointments = await _adminRepository.GetAllAppointmentsAsync();

            List<AdminAppointmentResponse> result = appointments.Select(x => new AdminAppointmentResponse
            {
                Id = x.Id,
                ServiceName = x.ProviderService.Name,
                Category = x.ProviderService.Category,
                ProviderName = x.ProviderService.Provider.CompanyName ?? (x.ProviderService.Provider.FirstName + " " + x.ProviderService.Provider.LastName).Trim(),
                ProviderId = x.ProviderService.ProviderId,
                ClientName = (x.Client.FirstName + " " + x.Client.LastName).Trim(),
                ClientId = x.ClientId,
                StartUtc = x.StartUtc,
                EndUtc = x.EndUtc,
                Status = x.Status.ToString(),
                PriceEur = x.ProviderService.PriceEur
            }).ToList();

            return ResultResponse<List<AdminAppointmentResponse>>.Ok(result);
        }

        public async Task<ResultResponse<AdminStatsResponse>> GetStatsAsync(DateTime? fromUtc, DateTime? toUtc)
        {
            List<User> users = await _adminRepository.GetAllUsersAsync();
            List<Appointment> appointments = await _adminRepository.GetAppointmentsInRangeAsync(fromUtc, toUtc);

            List<StatPointResponse> byDate = appointments
                .GroupBy(x => x.StartUtc.Date)
                .OrderBy(x => x.Key)
                .Select(x => new StatPointResponse
                {
                    Label = x.Key.ToString("yyyy-MM-dd"),
                    Value = x.Count()
                })
                .ToList();

            List<StatPointResponse> byCategory = appointments
                .GroupBy(x => x.ProviderService.Category)
                .OrderByDescending(x => x.Count())
                .Select(x => new StatPointResponse
                {
                    Label = x.Key,
                    Value = x.Count()
                })
                .ToList();

            AdminStatsResponse stats = new()
            {
                TotalUsers = users.Count(x => x.Role == UserRole.User),
                TotalProviders = users.Count(x => x.Role == UserRole.Provider),
                TotalAppointments = appointments.Count,
                ScheduledAppointments = appointments.Count(x => x.Status == AppointmentStatus.Scheduled),
                CompletedAppointments = appointments.Count(x => x.Status == AppointmentStatus.Completed),
                CancelledAppointments = appointments.Count(x => x.Status == AppointmentStatus.Cancelled),
                BookingsByDate = byDate,
                BookingsByCategory = byCategory
            };

            return ResultResponse<AdminStatsResponse>.Ok(stats);
        }
    }
}
