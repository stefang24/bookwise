using backend.DTOs;
using backend.Models;
using backend.Repositories.Interfaces;
using backend.Services.Interfaces;

namespace backend.Services
{
    public class WorkingHoursService : IWorkingHoursService
    {
        private readonly IWorkingHoursRepository _workingHoursRepository;
        private readonly IUserRepository _userRepository;

        public WorkingHoursService(IWorkingHoursRepository workingHoursRepository, IUserRepository userRepository)
        {
            _workingHoursRepository = workingHoursRepository;
            _userRepository = userRepository;
        }

        public async Task<ResultResponse<List<ProviderWorkingHourDto>>> GetByProviderAsync(int providerId)
        {
            List<ProviderWorkingHour> hours = await _workingHoursRepository.GetByProviderAsync(providerId);

            List<ProviderWorkingHourDto> data = hours
                .OrderBy(x => x.DayOfWeek)
                .Select(x => new ProviderWorkingHourDto
                {
                    DayOfWeek = x.DayOfWeek,
                    IsWorking = x.IsWorking,
                    StartTime = x.StartTime.HasValue ? x.StartTime.Value.ToString(@"hh\:mm") : null,
                    EndTime = x.EndTime.HasValue ? x.EndTime.Value.ToString(@"hh\:mm") : null
                })
                .ToList();

            return ResultResponse<List<ProviderWorkingHourDto>>.Ok(data);
        }

        public async Task<ResultResponse<List<ProviderWorkingHourDto>>> UpsertAsync(int providerId, UpdateWorkingHoursRequest request)
        {
            User? provider = await _userRepository.GetByIdAsync(providerId);
            if (provider == null || provider.Role != UserRole.Provider)
                return ResultResponse<List<ProviderWorkingHourDto>>.Fail("Provider not found.");

            if (request.Days.Count != 7)
                return ResultResponse<List<ProviderWorkingHourDto>>.Fail("Working hours must contain exactly 7 days.");

            List<ProviderWorkingHour> current = await _workingHoursRepository.GetByProviderAsync(providerId);
            Dictionary<int, ProviderWorkingHour> byDay = current.ToDictionary(x => x.DayOfWeek);

            foreach (ProviderWorkingHourDto day in request.Days)
            {
                if (day.DayOfWeek < 0 || day.DayOfWeek > 6)
                    return ResultResponse<List<ProviderWorkingHourDto>>.Fail("DayOfWeek must be in range 0-6.");

                TimeSpan? start = null;
                TimeSpan? end = null;

                if (day.IsWorking)
                {
                    if (!TimeSpan.TryParse(day.StartTime, out TimeSpan parsedStart) || !TimeSpan.TryParse(day.EndTime, out TimeSpan parsedEnd))
                        return ResultResponse<List<ProviderWorkingHourDto>>.Fail("StartTime and EndTime are required for working days.");

                    if (parsedEnd <= parsedStart)
                        return ResultResponse<List<ProviderWorkingHourDto>>.Fail("EndTime must be greater than StartTime.");

                    start = parsedStart;
                    end = parsedEnd;
                }

                if (!byDay.TryGetValue(day.DayOfWeek, out ProviderWorkingHour? existing))
                {
                    existing = new ProviderWorkingHour
                    {
                        ProviderId = providerId,
                        DayOfWeek = day.DayOfWeek
                    };
                    await _workingHoursRepository.AddAsync(existing);
                }

                existing.IsWorking = day.IsWorking;
                existing.StartTime = start;
                existing.EndTime = end;
            }

            await _workingHoursRepository.SaveChangesAsync();
            return await GetByProviderAsync(providerId);
        }
    }
}
