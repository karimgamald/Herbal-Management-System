using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Enums;

public enum PaymentStatus
{
    Pending = 1,    // لسه متدفعش (كاش أو لسه بيحول)
    AwaitingPayment, // دا لو هيدفع في حاله credit card or wallet بس
    Paid,       // تم الدفع (فيزا ) CreditCard
    Failed,     // عملية الدفع فشلت
    Refunded    // تم استرجاع الفلوس
}
