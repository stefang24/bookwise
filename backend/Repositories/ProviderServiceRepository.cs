using backend.Data;
using backend.Models;
using backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class ProviderServiceRepository : IProviderServiceRepository
    {
        private readonly AppDbContext _context;

        public ProviderServiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProviderService?> GetActiveByIdAsync(int id)
        {
            return await _context.ProviderServices
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        }

        public async Task<ProviderService?> GetActiveWithProviderByIdAsync(int id)
        {
            return await _context.ProviderServices
                .Include(x => x.Provider)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        }

        public async Task<ProviderService?> GetByIdAndProviderAsync(int serviceId, int providerId)
        {
            return await _context.ProviderServices
                .FirstOrDefaultAsync(x => x.Id == serviceId && x.ProviderId == providerId);
        }

        public async Task<ProviderService?> GetByIdAndProviderWithProviderAsync(int serviceId, int providerId)
        {
            return await _context.ProviderServices
                .Include(x => x.Provider)
                .FirstOrDefaultAsync(x => x.Id == serviceId && x.ProviderId == providerId);
        }

        public async Task<List<ProviderService>> GetByProviderActiveAsync(int providerId)
        {
            return await _context.ProviderServices
                .Include(x => x.Provider)
                .Where(x => x.ProviderId == providerId && x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<ProviderService>> GetTopBookedActiveAsync(int limit)
        {
            int safeLimit = Math.Max(1, limit);

            return await _context.ProviderServices
                .Include(x => x.Provider)
                .Where(x => x.IsActive && x.Provider.Role == UserRole.Provider)
                .OrderByDescending(x => x.Appointments.Count(a => a.Status != AppointmentStatus.Cancelled))
                .ThenBy(x => x.Name)
                .Take(safeLimit)
                .ToListAsync();
        }

        public async Task<List<ProviderService>> SearchActiveAsync(string? category, string? query)
        {
            IQueryable<ProviderService> services = _context.ProviderServices
                .Include(x => x.Provider)
                .Where(x => x.IsActive && x.Provider.Role == UserRole.Provider);

            if (!string.IsNullOrWhiteSpace(category))
            {
                string normalizedCategory = category.Trim().ToLower();
                services = services.Where(x => x.Category.ToLower() == normalizedCategory);
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                string normalizedQuery = query.Trim().ToLower();
                services = services.Where(x =>
                    x.Name.ToLower().Contains(normalizedQuery) ||
                    (x.Provider.CompanyName != null && x.Provider.CompanyName.ToLower().Contains(normalizedQuery)) ||
                    x.Provider.Username.ToLower().Contains(normalizedQuery));
            }

            return await services
                .OrderBy(x => x.Category)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<string>> GetActiveCategoriesAsync()
        {
            return await _context.ProviderServices
                .Where(x => x.IsActive && !string.IsNullOrWhiteSpace(x.Category))
                .Select(x => x.Category.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
        }

        public async Task AddAsync(ProviderService entity)
        {
            _context.ProviderServices.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
