namespace PhytoIntellect.Application.DTOs.HerbalistDTOs
{
    public class CreateOrUpdateHerbalistDto
    {
        //public string LicenseNumber { get; set; } = string.Empty;
        // those attributes are nullable, because the herbalist created at register first
        public string? Bio { get; set; }
        public TimeSpan? AvailableFrom { get; set; }
        public TimeSpan? AvailableTo { get; set; }
    }
}
