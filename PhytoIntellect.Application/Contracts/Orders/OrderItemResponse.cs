using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

// 5. تفاصيل العنصر جوه الطلب
public record OrderItemResponse
{
    public int? ItemId { get; init; }
    public string Name { get; init; } = string.Empty;
    public int? Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal? SubTotal { get; init; }
}