using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(x => x.ProviderService)
                    .ThenInclude(x => x.Provider)
                .Include(x => x.Client)
                .OrderByDescending(x => x.StartUtc)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsInRangeAsync(DateTime? fromUtc, DateTime? toUtc)
        {
            IQueryable<Appointment> query = _context.Appointments
                .Include(x => x.ProviderService)
                    .ThenInclude(x => x.Provider)
                .Include(x => x.Client);

            if (fromUtc.HasValue)
                query = query.Where(x => x.StartUtc >= fromUtc.Value);

            if (toUtc.HasValue)
                query = query.Where(x => x.StartUtc <= toUtc.Value);

            return await query.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
