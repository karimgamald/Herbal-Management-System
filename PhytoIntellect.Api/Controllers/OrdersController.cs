using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Api.Extensions;
using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Patient)]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;

    [HttpGet("all-my-orders")]
    public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var orders = await _orderService.GetPatientOrdersAsync(userId!, cancellationToken);
        return Ok(orders);
    }

    [HttpGet("{orderId}/get-id")]
    public async Task<IActionResult> GetOrderById(int orderId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized(new { Message = "User is not logged in." });

            var orderDetails = await orderService.GetOrderDetailsForPatientAsync(orderId, userId, cancellationToken);

            if (orderDetails == null)
            {
                return NotFound(new { Message = "Order not found or you do not have permission to view it." });
            }

            return Ok(orderDetails);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while fetching the order details.", Details = ex.Message });
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var result = await _orderService.CreateOrderAsync(userId, request, cancellationToken);
            return Ok(new { Message = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { ex.Message });
        }
    }

    [HttpPut("{orderId}/cancel")]
    public async Task<IActionResult> CancelOrder(int orderId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        try
        {
            await _orderService.CancelOrderAsync(orderId, userId!, cancellationToken);
            return Ok(new { Message = "Order cancelled successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("{orderId}/simulate-payment")]
    public async Task<IActionResult> SimulatePayment(int orderId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized(new { Message = "User is not logged in." });

            var transactionId = await orderService.SimulatePaymentAsync(orderId, userId, cancellationToken);

            return Ok(new
            {
                Message = "Payment simulated successfully. Order is now pending.",
                TransactionId = transactionId
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
        }
    }

    [HttpPatch("{id}/favorite")]
    public async Task<IActionResult> ToggleFavorite(int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId().ToString();
        var isFavorite = await _orderService.ToggleFavoriteOrderAsync(userId, id, cancellationToken);

        return Ok(new
        {
            Message = isFavorite ? "Order added to favorites." : "Order removed from favorites.",
            IsFavorite = isFavorite
        });
    }

    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavoriteOrders(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId().ToString();
        var result = await _orderService.GetFavoriteOrdersAsync(userId, cancellationToken);
        return Ok(result);
    }
}