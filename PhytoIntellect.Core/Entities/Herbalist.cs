using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class Herbalist
{
    public int HerbalistId { get; set; }

    public int UserId { get; set; }

    public string LicenseNumber { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public string Bio { get; set; } = string.Empty;
    public TimeSpan AvailableFrom { get; set; } 
    public TimeSpan AvailableTo { get; set; }

    // Navigation Property
    public User? User { get; set; }
}
