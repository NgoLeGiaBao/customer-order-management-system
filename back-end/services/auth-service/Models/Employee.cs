using Microsoft.AspNetCore.Identity;

namespace auth_service.Models
{
    public class Employee : IdentityUser<int>
{
     public required string FullName { get; set; }
    public int RoleId { get; set; }
    public virtual Role Role { get; set; }
}
}
