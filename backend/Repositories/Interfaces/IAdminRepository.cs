using backend.Models;

namespace backend.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<List<Appointment>> GetAllAppointmentsAsync();
        Task<List<Appointment>> GetAppointmentsInRangeAsync(DateTime? fromUtc, DateTime? toUtc);
        Task SaveChangesAsync();
    }
}
