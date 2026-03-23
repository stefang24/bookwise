using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<List<User>> GetProvidersFilteredAsync(string? category, string? city, string? query)
        {
            IQueryable<User> providers = _context.Users
                .Include(x => x.ProviderServices.Where(s => s.IsActive))
                .Where(x => x.Role == UserRole.Provider)
                .Where(x => x.ProviderServices.Any(s => s.IsActive));

            if (!string.IsNullOrWhiteSpace(category))
            {
                string normalizedCategory = category.Trim().ToLower();
                providers = providers.Where(x =>
                    (x.PrimaryCategory != null && x.PrimaryCategory.ToLower() == normalizedCategory) ||
                    x.ProviderServices.Any(s => s.IsActive && s.Category.ToLower() == normalizedCategory));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                string normalizedCity = city.Trim().ToLower();
                providers = providers.Where(x => x.City != null && x.City.ToLower() == normalizedCity);
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                string normalizedQuery = query.Trim().ToLower();
                providers = providers.Where(x =>
                    x.Username.ToLower().Contains(normalizedQuery) ||
                    (x.CompanyName != null && x.CompanyName.ToLower().Contains(normalizedQuery)) ||
                    x.FirstName.ToLower().Contains(normalizedQuery));
            }

            return await providers.ToListAsync();
        }

        public async Task<List<User>> GetTopProvidersByAppointmentsAsync(int limit)
        {
            int safeLimit = Math.Max(1, limit);

            return await _context.Users
                .Include(x => x.ProviderServices.Where(s => s.IsActive))
                .Where(x => x.Role == UserRole.Provider)
                .Where(x => x.ProviderServices.Any(s => s.IsActive))
                .OrderByDescending(x => x.ProviderServices
                    .SelectMany(s => s.Appointments)
                    .Count(a => a.Status != AppointmentStatus.Cancelled))
                .ThenBy(x => x.CompanyName ?? (x.FirstName + " " + x.LastName).Trim())
                .Take(safeLimit)
                .ToListAsync();
        }
    }
}
