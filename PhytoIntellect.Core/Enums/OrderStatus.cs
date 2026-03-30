using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Enums;

public enum OrderStatus
{
    Pending = 1,              // قيد الانتظار
    Processing = 2,           // جاري التجهيز
    Shipped = 3,              // تم الشحن (كل العطارين شحنوا)
    PartiallyShipped = 4,     // تم الشحن جزئياً (عطار شحن وعطار لغى) 👈 الجديد
    Delivered = 5,            // تم التوصيل (الطلب كامل)
    PartiallyDelivered = 6,   // تم التوصيل جزئياً (الطلب ناقص) 👈 الجديد
    Cancelled = 7             // تم الإلغاء (كل العطارين لغوا أو المريض لغى)
}