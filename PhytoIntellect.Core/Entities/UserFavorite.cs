using PhytoIntellect.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class UserFavorite
{
    public int Id { get; set; } // لو عندك BaseEntity بتورث منه شيل السطر ده

    public int UserId { get; set; }
    public User User { get; set; } = null!; // Navigation Property

    public int TargetId { get; set; } // رقم الحاجة (عشبة أو وصفة)
    public FavoriteType Type { get; set; } // نوعها

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}