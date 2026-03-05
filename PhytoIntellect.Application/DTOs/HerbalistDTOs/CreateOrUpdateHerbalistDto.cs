using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.HerbalistDTOs
{
    public class CreateOrUpdateHerbalistDto
    {
        public string LicenseNumber { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        public TimeSpan AvailableFrom { get; set; }
        public TimeSpan AvailableTo { get; set; }
    }
}
