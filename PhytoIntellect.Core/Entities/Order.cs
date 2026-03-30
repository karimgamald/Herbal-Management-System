using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class Order
{
    public int OrderId { get; set; }
    public int PatientId { get; set; }

    // الفلوس
    public decimal ItemsTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal TotalPrice { get; set; }

    // الدفع
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = "Pending";
    public string? ExternalPaymentID { get; set; }

    // الشحن
    public string ShippingAddress { get; set; } = string.Empty;

    // التواريخ والحالة
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string OrderStatus { get; set; } = "Pending";

    // Navigation Properties
    public Patient? Patient { get; set; }
    public ICollection<SubOrder> SubOrders { get; set; } = [];
}