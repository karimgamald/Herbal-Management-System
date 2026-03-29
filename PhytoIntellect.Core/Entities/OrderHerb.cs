using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class OrderHerb
{
    public int OrderHerbId { get; set; }
    public int SubOrderId { get; set; }
    public int? HerbId { get; set; }

    public int? Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? SubTotal { get; set; }

    // Navigation Properties
    public SubOrder? SubOrder { get; set; }
    public Herb? Herb { get; set; }
}
