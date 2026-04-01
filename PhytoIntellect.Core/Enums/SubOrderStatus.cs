using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Enums;

public enum SubOrderStatus
{
    Pending = 1,    // في انتظار رد العطار (الأوردر لسه واصل)
    Preparing = 2, // العطار وافق وبدأ يجهز الأعشاب 
    Shipped = 3,    // العطار سلم الجزء بتاعه لشركة الشحن --- Exteranl delivery Id = value
    Delivered = 4,  // الجزء ده وصل للمريض بنجاح
    Cancelled = 5   // العطار لغى الطلب (الكمية خلصت مثلاً)
}