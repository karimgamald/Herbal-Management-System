using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public abstract class LocalizedEntity
{
    public string LanguageCode { get; set; } = "en";
}