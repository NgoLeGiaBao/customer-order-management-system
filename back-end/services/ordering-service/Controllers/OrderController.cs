using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using order_service.Data;
using order_service.Enums;

namespace order_service.Controllers
{
    [Route("orders")]
    [ApiController]
    [Authorize] 
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all orders
        [HttpGet("get-all-orders")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetOrders()
        {
            var orders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Closed) 
                .Select(o => new
                {
                    o.Id,
                    o.TableId,
                    o.EmployeeId,
                    o.Status,
                    o.CreatedAt,
                    TotalAmount = o.Items
                        .Where(i => i.Status == OrderItemStatus.Served) 
                        .GroupBy(i => i.MenuItemId)
                        .Select(g => g.Sum(i => i.Price * i.Quantity)) 
                        .Sum(), 
                    Items = o.Items
                        .Where(i => i.Status == OrderItemStatus.Served)
                        .GroupBy(i => i.MenuItemId) 
                        .Select(g => new
                        {
                            MenuItemId = g.Key,
                            Name = g.First().MenuItemName, 
                            Quantity = g.Sum(i => i.Quantity), 
                            TotalPrice = g.Sum(i => i.Price * i.Quantity) 
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }



        // Get order by ID (Admin & Waiter)
        [HttpGet("get-order-by-id/{id}")]
        [Authorize(Roles = "Admin,Waiter")]
        public async Task<ActionResult<object>> GetOrderById(int id)
        {
            var order = await _context.Orders
                .Where(o => o.Id == id)
                .Select(o => new
                {
                    o.Id,
                    o.TableId,
                    o.EmployeeId,
                    o.Status,
                    o.CreatedAt,
                    TotalAmount = o.Items
                        .Where(i => i.Status == OrderItemStatus.Served) 
                        .GroupBy(i => i.MenuItemId)
                        .Select(g => g.Sum(i => i.Price * i.Quantity))
                        .Sum(),
                    Items = o.Items
                        .Where(i => i.Status == OrderItemStatus.Served)
                        .GroupBy(i => i.MenuItemId)
                        .Select(g => new
                        {
                            MenuItemId = g.Key,
                            Name = g.First().MenuItemName, 
                            Quantity = g.Sum(i => i.Quantity), 
                            TotalPrice = g.Sum(i => i.Price * i.Quantity) 
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
    }
}
