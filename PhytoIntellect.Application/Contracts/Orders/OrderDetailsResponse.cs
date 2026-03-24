using PhytoIntellect.Application.Contracts.SubOrders;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

// 2. تفاصيل طلب المريض (عشان الـ Details)
public record OrderDetailsResponse
{
    public int OrderId { get; init; }
    public decimal ItemsTotal { get; init; }
    public decimal DeliveryFee { get; init; }
    public decimal TotalPrice { get; init; }
    public string ShippingAddress { get; init; } = string.Empty;
    public string OrderStatus { get; init; } = string.Empty;
    public DateTime OrderDate { get; init; }
    public List<SubOrderSummaryResponse> SubOrders { get; init; } = new();
}