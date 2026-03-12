namespace backend.DTOs
{
    public class ProfileResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; }
        public string? Bio { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyDescription { get; set; }
        public string? Address { get; set; }
        public string? Website { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
