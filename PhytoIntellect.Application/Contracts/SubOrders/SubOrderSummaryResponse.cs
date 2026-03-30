using PhytoIntellect.Application.Contracts.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.SubOrders;
// 3. ملخص الطلب الفرعي (للعطار وللمريض)
// 2. 👈 التعديل هنا: ضفنا بيانات العطار للـ SubOrder
public record SubOrderSummaryResponse
{
    public int SubOrderId { get; init; }
    //public int HerbalistId { get; init; }
    //public string HerbalistName { get; init; } = string.Empty;
    public decimal SubTotal { get; init; }
    public string Status { get; init; } = string.Empty;
    //public string? TrackingNumber { get; init; }
    //public List<OrderItemResponse> Items { get; init; } = new();
}