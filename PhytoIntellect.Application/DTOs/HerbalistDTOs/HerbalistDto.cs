namespace PhytoIntellect.Application.DTOs.HerbalistDTOs;

public class HerbalistDto
{
    public int HerbalistId { get; set; }
    public int UserId { get; set; }

    public string LicenseNumber { get; set; } = string.Empty;
    public double AverageRating { get; set; }

    public string Bio { get; set; } = string.Empty;

    public TimeSpan AvailableFrom { get; set; }
    public TimeSpan AvailableTo { get; set; }
}