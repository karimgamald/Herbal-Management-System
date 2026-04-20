using PhytoIntellect.Application.Contracts.SubOrders;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

public record OrderDetailsResponse
{
    public int OrderId { get; init; }
    public decimal ItemsTotal { get; init; }
    public decimal DeliveryFee { get; init; }
    public decimal TotalPrice { get; init; }
    public string ShippingAddress { get; init; } = string.Empty;
    public string PaymentMethod { get; init; } = string.Empty;
    public string PaymentStatus { get; init; } = string.Empty;
    public string? TransactionId { get; init; }
    public string OrderStatus { get; init; } = string.Empty;
    public DateTime OrderDate { get; init; }
    public bool IsFavorite { get; init; }

    public List<SubOrderDetailsResponse> SubOrders { get; init; } = new();
}