using PhytoIntellect.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Services;

public class CurrentLanguageService : ICurrentLanguageService
{
    public string LanguageCode { get; set; } = "en";
}