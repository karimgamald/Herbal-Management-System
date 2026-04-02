using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Inventory;

public class UpdateInventoryRequest
{
    public decimal PricePerKilo { get; set; }

    public bool IsActive { get; set; }
}
