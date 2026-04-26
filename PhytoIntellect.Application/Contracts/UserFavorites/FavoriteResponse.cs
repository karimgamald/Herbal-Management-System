using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.UserFavorites;

public class FavoriteResponse
{
    public int TargetId { get; set; }
    public string Name { get; set; } = string.Empty;

}