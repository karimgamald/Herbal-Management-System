using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface ICurrentLanguageService
{
    string LanguageCode { get; set; }
}