using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class HerbalistHerb // inventory
{
    // Composite Key (مفيش Id برايمري لوحده، الاتنين دول مع بعض هما البرايمري)
    public int HerbalistId { get; set; }
    public int HerbId { get; set; }

    // هنا نقدر نحط السعر، عشان كل عطار يبيع العشبة العامة دي بسعره الخاص (Marketplace صح)
    public decimal? Price { get; set; }

    // هل العشبة دي متاحة عنده دلوقتي للبيع ولا خلصانة؟
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Herbalist Herbalist { get; set; } = null!;
    public Herb Herb { get; set; } = null!;
}