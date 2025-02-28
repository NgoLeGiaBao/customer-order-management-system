using Microsoft.AspNetCore.Identity;

namespace auth_service.Models
{
    public class Role : IdentityRole<int>
    {
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}