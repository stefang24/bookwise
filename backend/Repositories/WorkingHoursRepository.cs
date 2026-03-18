using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class WorkingHoursRepository : IWorkingHoursRepository
    {
        private readonly AppDbContext _context;

        public WorkingHoursRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProviderWorkingHour>> GetByProviderAsync(int providerId)
        {
            return await _context.ProviderWorkingHours
                .Where(x => x.ProviderId == providerId)
                .OrderBy(x => x.DayOfWeek)
                .ToListAsync();
        }

        public async Task<ProviderWorkingHour?> GetByProviderAndDayAsync(int providerId, int dayOfWeek)
        {
            return await _context.ProviderWorkingHours
                .FirstOrDefaultAsync(x => x.ProviderId == providerId && x.DayOfWeek == dayOfWeek);
        }

        public Task AddAsync(ProviderWorkingHour entity)
        {
            _context.ProviderWorkingHours.Add(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
