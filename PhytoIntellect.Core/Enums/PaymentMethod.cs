using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Enums;

public enum PaymentMethod
{
    Cash = 1,   // الدفع عند الاستلام
    CreditCard = 2,       // فيزا / ماستركارد
    Wallet = 3,     // فودافون كاش / المحافظ الإلكترونية
}