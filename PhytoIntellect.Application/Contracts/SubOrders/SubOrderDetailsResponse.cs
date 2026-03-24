using PhytoIntellect.Application.Contracts.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.SubOrders;

// 4. تفاصيل الطلب الفرعي (للعطار عشان يجهز الشغل)
public record SubOrderDetailsResponse
{
    public int SubOrderId { get; init; }
    public decimal SubTotal { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? TrackingNumber { get; init; }
    public List<OrderItemResponse> Recipes { get; init; } = new();
    public List<OrderItemResponse> Herbs { get; init; } = new();
}