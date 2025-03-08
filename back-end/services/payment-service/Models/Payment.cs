using payment_service.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace payment_service.Models
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; } 

        [Required]
        public PaymentMethod PaymentMethod { get; set; } 

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Unpaid; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    }
}