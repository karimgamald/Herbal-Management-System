using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class Herbalist
{
    public int HerbalistId { get; set; }

    public int UserId { get; set; }

    public string LicenseNumber { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public string? Bio { get; set; }
    public TimeSpan? AvailableFrom { get; set; } 
    public TimeSpan? AvailableTo { get; set; }

    // Navigation Property
    public User? User { get; set; }

    // 2. فاترينة العطار (الأعشاب اللي بيبيعها ومأكتفها عنده)
    public ICollection<HerbalistHerb> HerbalistHerbs { get; set; } = [];

    // 3. الوصفات اللي العطار ده ألفها أو عملها
    public ICollection<Recipe> Recipes { get; set; } = [];

    // 4. (مستقبلاً) الطلبات الفرعية اللي مطلوبة من العطار ده عشان يجهزها
    // public ICollection<SubOrder> SubOrders { get; set; } = new List<SubOrder>();
}
