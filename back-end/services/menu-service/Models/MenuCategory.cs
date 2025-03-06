using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace menu_service.Models
{
    public class MenuCategory
    {
        [Key]
        public int Id { get; set; }  

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }
        public bool IsAvailable { get; set; } = true;

        public List<MenuItem>? MenuItems { get; set; } = new();
    }
}
