using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Enums;

public enum SubOrderStatus
{
    Pending = 1,// في انتظار رد العطار (الأوردر لسه واصل)
    AwaitingPayment,
    Preparing, // العطار وافق وبدأ يجهز الأعشاب 
    Shipped,    // العطار سلم الجزء بتاعه لشركة الشحن --- Exteranl delivery Id = value
    Delivered,  // الجزء ده وصل للمريض بنجاح
    Cancelled   // العطار لغى الطلب (الكمية خلصت مثلاً)
}