using backend.Models;

namespace backend.Repositories
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetPastScheduledAsync();
        Task<List<Appointment>> GetScheduledByProviderForDateAsync(int providerId, DateTime date);
        Task<bool> HasConflictAsync(int providerId, DateTime startUtc, DateTime endUtc);
        Task<Appointment?> GetByIdWithServiceAsync(int id);
        Task<List<Appointment>> GetHistoryAsync(int userId, bool isProvider);
        Task AddAsync(Appointment entity);
        Task SaveChangesAsync();
    }
}
