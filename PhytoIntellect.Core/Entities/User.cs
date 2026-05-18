namespace PhytoIntellect.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsEmailConfirmed { get; set; } = false;
        public string? EmailConfirmationToken { get; set; }
        public DateTime? EmailConfirmationTokenExpiry { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Phone { get; set; } = string.Empty;
        public string? Governorate { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }

        // Navigation Properties
        public Patient? Patient { get; set; }
        public Herbalist? Herbalist { get; set; }
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
