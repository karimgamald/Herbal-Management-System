using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Herbs
{
    public class HerbWithHerbalistResponse
    {
        public int HerbId { get; set; }
        public string HerbName { get; set; } = string.Empty;
        public string HerbalistName { get; set; } = string.Empty; // اسم العطار
        public int HerbalistId { get; set; }
        public string? ImageURL { get; set; }
    }
}
