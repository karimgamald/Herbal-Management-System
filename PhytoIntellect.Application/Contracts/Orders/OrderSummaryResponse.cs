using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;
public record OrderSummaryResponse(
    int OrderId,
    decimal TotalPrice,
    string OrderStatus,
    DateTime OrderDate,
    string PaymentStatus);