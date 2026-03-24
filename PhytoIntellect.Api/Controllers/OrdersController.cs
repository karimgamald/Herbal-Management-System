using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;
using PhytoIntellect.Core.Entities;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Patient)]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            // بنكلم الـ Service
            var result = await _orderService.CreateOrderAsync(userId, request, cancellationToken);
            return Ok(new { Message = result });
        }
        catch (InvalidOperationException ex)
        {
            // 🎯 هنا بنمسك إيرور الـ AI أو إيرور الأعشاب اللي ملهاش عطار، وبنرد بـ 400
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            // أي إيرور تاني مش متوقع (زي الداتابيز وقعت) بنرجعه 500
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var orders = await _orderService.GetPatientOrdersAsync(userId!, cancellationToken);
        return Ok(orders);
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrderDetails(int orderId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var orderDetails = await _orderService.GetOrderDetailsForPatientAsync(orderId, userId!, cancellationToken);
        return Ok(orderDetails);
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
}