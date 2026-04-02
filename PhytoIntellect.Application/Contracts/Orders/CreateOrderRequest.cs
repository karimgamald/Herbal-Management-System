
namespace PhytoIntellect.Application.Contracts.Orders;

public record CreateOrderRequest
{
    public string ShippingAddress { get; init; } = string.Empty;
    public string PaymentMethod { get; init; } = string.Empty;

    public List<OrderRecipeRequest> Recipes { get; init; } = [];
    public List<OrderHerbRequest> Herbs { get; init; } = [];
}