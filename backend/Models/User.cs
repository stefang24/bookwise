namespace backend.Models
{
    public enum UserRole
    {
        User,
        Provider,
        Admin
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        public string? ProfileImagePath { get; set; }
        public string? Bio { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CompanyName { get; set; }
        public string? PrimaryCategory { get; set; }
        public string? CompanyDescription { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Website { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ProviderService> ProviderServices { get; set; } = [];
        public ICollection<ProviderWorkingHour> WorkingHours { get; set; } = [];
        public ICollection<Appointment> ClientAppointments { get; set; } = [];
    }
}
