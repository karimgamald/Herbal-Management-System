using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Enums;

public enum PaymentStatus
{
    Pending = 1,    // لسه متدفعش (كاش أو لسه بيحول)
    Paid = 2,       // تم الدفع (فيزا ) CreditCard
    Failed = 3,     // عملية الدفع فشلت
    Refunded = 4    // تم استرجاع الفلوس
}
