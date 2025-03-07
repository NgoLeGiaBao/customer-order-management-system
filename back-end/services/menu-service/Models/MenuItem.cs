using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; 

namespace menu_service.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public int CategoryId { get; set; }  

        [ForeignKey("CategoryId")]
        [JsonIgnore] 
        public MenuCategory? Category { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal CurrentPrice { get; set; }

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
