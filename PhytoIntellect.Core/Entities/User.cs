namespace PhytoIntellect.Core.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; } // 1 for Patient, 2 for Herbalist
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Phone { get; set; }
        public string? Governorate { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }

        // Navigation Properties
        public Patient Patient { get; set; }
        public Herbalist Herbalist { get; set; }
    }
}
