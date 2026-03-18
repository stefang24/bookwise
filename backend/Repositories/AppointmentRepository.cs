using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetPastScheduledAsync()
        {
            DateTime nowUtc = DateTime.UtcNow;
            return await _context.Appointments
                .Where(x => x.Status == AppointmentStatus.Scheduled && x.EndUtc <= nowUtc)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetScheduledByProviderForDateAsync(int providerId, DateTime date)
        {
            return await _context.Appointments
                .Where(x => x.ProviderService.ProviderId == providerId && x.Status == AppointmentStatus.Scheduled)
                .Where(x => x.StartUtc.Date == date.Date)
                .ToListAsync();
        }

        public async Task<bool> HasConflictAsync(int providerId, DateTime startUtc, DateTime endUtc)
        {
            return await _context.Appointments
                .AnyAsync(x => x.ProviderService.ProviderId == providerId &&
                               x.Status == AppointmentStatus.Scheduled &&
                               x.StartUtc < endUtc &&
                               x.EndUtc > startUtc);
        }

        public async Task<Appointment?> GetByIdWithServiceAsync(int id)
        {
            return await _context.Appointments
                .Include(x => x.ProviderService)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Appointment>> GetHistoryAsync(int userId, bool isProvider)
        {
            IQueryable<Appointment> query = _context.Appointments
                .Include(x => x.ProviderService)
                    .ThenInclude(x => x.Provider)
                .Include(x => x.Client);

            query = isProvider
                ? query.Where(x => x.ProviderService.ProviderId == userId)
                : query.Where(x => x.ClientId == userId);

            return await query.OrderByDescending(x => x.StartUtc).ToListAsync();
        }

        public Task AddAsync(Appointment entity)
        {
            _context.Appointments.Add(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
