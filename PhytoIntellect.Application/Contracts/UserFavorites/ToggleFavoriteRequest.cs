using PhytoIntellect.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.UserFavorites;

public class ToggleFavoriteRequest
{
    public int TargetId { get; set; }
    public string Type { get; set; } = string.Empty;
}