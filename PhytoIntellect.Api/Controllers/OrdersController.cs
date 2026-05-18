using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Api.Extensions;
using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Constants;
using System.Security.Claims;

namespace PhytoIntellect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService _orderService) : ControllerBase
{

    [Authorize(Roles = AppRoles.Patient)]
    [HttpGet("all-my-orders")]
    public async Task<IActionResult> GetMyOrders([FromQuery] RequestFilters filters,CancellationToken cancellationToken)
    {
        var userId = User.GetUserId().ToString();
        var orders = await _orderService.GetPatientOrdersAsync(userId!, filters,cancellationToken);
        return Ok(orders);
    }

    [Authorize(Roles = AppRoles.Patient)]
    [HttpGet("{id}/get-id")]
    public async Task<IActionResult> GetOrderById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetUserId().ToString();
            if (userId == null) return Unauthorized(new { Message = "User is not logged in." });

            var orderDetails = await _orderService.GetOrderDetailsForPatientAsync(id, userId, cancellationToken);

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

    [Authorize(Roles = AppRoles.Patient)]
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId().ToString();

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

    [Authorize(Roles = $"{AppRoles.Patient},{AppRoles.Admin}")]
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId().ToString();

        try
        {
            await _orderService.CancelOrderAsync(id, userId!, cancellationToken);
            return Ok(new { Message = "Order cancelled successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [Authorize(Roles = AppRoles.Patient)]
    [HttpPut("{id}/simulate-payment")]
    public async Task<IActionResult> SimulatePayment(int id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetUserId().ToString();
            if (userId == null) return Unauthorized(new { Message = "User is not logged in." });

            var transactionId = await _orderService.SimulatePaymentAsync(id, userId, cancellationToken);

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

    [Authorize(Roles = AppRoles.Patient)]
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

    [Authorize(Roles = AppRoles.Patient)]
    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavoriteOrders([FromQuery] RequestFilters filters,CancellationToken cancellationToken)
    {
        var userId = User.GetUserId().ToString();
        var result = await _orderService.GetFavoriteOrdersAsync(userId, filters,cancellationToken);
        return Ok(result);
    }

    // New Endpoint: Get only orders that are Pending and haven't been touched by herbalists yet
    // متاح فقط للمسؤول (Admin) لعرض جميع الطلبات المعلقة بالنظام
    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("pending-unapproved")]
    public async Task<IActionResult> GetPendingUnapprovedOrders([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        try
        {
            // استدعاء الدالة المخصصة للـ Admin لجلب كل الطلبات المعلقة دون التقييد بـ UserId معين
            var orders = await _orderService.GetAllPendingOrdersForAdminAsync(filters, cancellationToken);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occured when getting data for Admin.", Details = ex.Message });
        }
    }

    // New Endpoint: Get ALL orders in the entire system for Admin management
    // متاح فقط للمسؤول (Admin) لمشاهدة وإدارة جميع الطلبات في النظام
    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("admin/all-orders")]
    public async Task<IActionResult> GetAllOrdersForAdmin([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        try
        {
            // استدعاء الخدمة لجلب كل الطلبات دون أي قيود على الحالة أو المستخدم
            var orders = await _orderService.GetAllOrdersForAdminAsync(filters, cancellationToken);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occured when getting data for Admin.", Details = ex.Message });
        }
    }
}