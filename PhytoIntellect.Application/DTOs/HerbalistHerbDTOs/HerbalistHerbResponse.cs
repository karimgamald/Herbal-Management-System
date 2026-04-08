using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.HerbalistHerbDTOs
{
    public class HerbalistHerbResponse
    {
        public int HerbalistId { get; set; }
        public string HerbalistName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public decimal Price { get; set; }
    }
}
