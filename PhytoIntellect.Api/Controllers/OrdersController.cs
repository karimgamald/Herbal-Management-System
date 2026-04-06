using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            // 1. استخراج الـ ID بتاع المريض من التوكن للأمان
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized(new { Message = "User is not logged in." });

            // 2. بنكلم السيرفيس اللي ظبطناها بالـ Includes والـ AutoMapper
            var orderDetails = await orderService.GetOrderDetailsForPatientAsync(orderId, userId, cancellationToken);

            // 3. لو السيرفيس رجعت null، ده معناه إن الأوردر مش موجود أو مش بتاع المريض ده
            if (orderDetails == null)
            {
                return NotFound(new { Message = "Order not found or you do not have permission to view it." });
            }

            // 4. لو كله تمام، نرجع الفاتورة بالتفاصيل
            return Ok(orderDetails);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while fetching the order details.", Details = ex.Message });
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
            // 👈 استخراج الـ ID بتاع المريض من التوكن عشان الأمان
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized(new { Message = "User is not logged in." });

            // 👈 بنبعت الـ userId للسيرفيس
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
            // 👈 لو حاول يدفع أوردر مش بتاعه هيرجعله 404 (كان الأوردر مش موجود أصلاً)
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


}