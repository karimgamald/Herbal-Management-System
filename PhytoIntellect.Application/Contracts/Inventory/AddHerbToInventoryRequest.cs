using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Inventory;

public class AddHerbToInventoryRequest
{
    public int HerbId { get; set; }

    public decimal PricePerKilo { get; set; }
}
