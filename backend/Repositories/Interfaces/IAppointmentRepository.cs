using backend.Models;

namespace backend.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetPastScheduledAsync();
        Task<List<Appointment>> GetScheduledByProviderForDateAsync(int providerId, DateOnly date);
        Task<bool> HasConflictAsync(int providerId, DateTime startUtc, DateTime endUtc);
        Task<Appointment?> GetByIdWithServiceAsync(int id);
        Task<List<Appointment>> GetHistoryAsync(int userId, bool isProvider);
        Task AddAsync(Appointment entity);
        Task SaveChangesAsync();
    }
}
