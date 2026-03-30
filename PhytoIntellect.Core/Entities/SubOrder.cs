using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class SubOrder
{
    public int SubOrderId { get; set; }
    public int OrderId { get; set; }
    public int HerbalistId { get; set; }

    public decimal SubTotal { get; set; }
    public string Status { get; set; } = "Pending";
    public string? ExternalDeliveryID { get; set; }
    
    // Navigation Properties
    public Order? Order { get; set; }
    public Herbalist? Herbalist { get; set; }
    public ICollection<OrderRecipe> OrderRecipes { get; set; } = [];
    public ICollection<OrderHerb> OrderHerbs { get; set; } = [];
}