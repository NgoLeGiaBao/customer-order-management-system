using Microsoft.AspNetCore.Mvc;
using order_service.Data;
using order_service.Models;
using order_service.Enums;
using order_service.Dtos;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace order_service.Controllers {
    [Route("order-items")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        // Add order items
        [HttpPost("add-order-items")]
        public async Task<IActionResult> AddOrderItems([FromBody] List<OrderItemDto> items)
        {
            if (items == null || !items.Any())
                return BadRequest("Invalid order items list.");

            var orderItems = items.Select(item => new OrderItem
            {
                OrderId = item.OrderId,
                MenuItemId = item.MenuItemId,
                MenuItemName = item.MenuItemName,
                Quantity = item.Quantity,
                Price = item.Price,
                Status = OrderItemStatus.Pending, 
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _context.OrderItems.AddRange(orderItems);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order items added successfully." });
        }

        
    }
}