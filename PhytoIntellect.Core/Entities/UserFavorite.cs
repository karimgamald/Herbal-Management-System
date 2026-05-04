using PhytoIntellect.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class UserFavorite
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int TargetId { get; set; }
    public FavoriteType Type { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}