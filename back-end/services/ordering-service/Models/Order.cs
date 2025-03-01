using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using order_service.Enums;

namespace order_service.Models;

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int TableId { get; set; }

    [Required]
    public int EmployeeId { get; set; }     

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Open;

    public List<OrderItem> Items { get; set; } = new();
}
