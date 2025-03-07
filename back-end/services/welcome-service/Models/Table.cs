using System.ComponentModel.DataAnnotations;
using WelcomeService.Enums;

namespace WelcomeService.Models
{
    public class Table
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TableNumber { get; set; }

        [Required]
        public TableStatus Status { get; set; } = TableStatus.Available;

        [Required]
        public int Capacity { get; set; }
    }
}
