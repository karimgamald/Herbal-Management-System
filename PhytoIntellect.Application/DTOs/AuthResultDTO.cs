using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs
{
    namespace PhytoIntellect.Application.DTOs
    {
        public class AuthResultDTO
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public object? Data { get; set; }
        }
    }
}
