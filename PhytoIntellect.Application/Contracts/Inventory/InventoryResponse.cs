using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Inventory;

public class InventoryResponse
{
    public int HerbId { get; set; }

    public string HerbName { get; set; } = string.Empty;

    public decimal? Price { get; set; }

    public bool IsActive { get; set; }
}
