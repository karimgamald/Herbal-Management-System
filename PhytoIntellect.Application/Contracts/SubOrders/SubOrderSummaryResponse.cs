using PhytoIntellect.Application.Contracts.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.SubOrders;
public record SubOrderSummaryResponse
{
    public int SubOrderId { get; init; }
    public decimal SubTotal { get; init; }
    public string Status { get; init; } = string.Empty;
}