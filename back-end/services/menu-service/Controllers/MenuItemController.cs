using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using menu_service.Data;
using menu_service.Models;
using Microsoft.EntityFrameworkCore;

namespace menu_service.Controllers
{
    [Route("menu-item")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MenuItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("get-all-items")]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetMenuItems()
        {
            return await _context.MenuItems.Include(m => m.Category).ToListAsync();
        }

        [HttpGet("get-item-by-id/{id}")]
        public async Task<ActionResult<MenuItem>> GetMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.Include(m => m.Category).FirstOrDefaultAsync(m => m.Id == id);
            if (menuItem == null)
                return NotFound();
            return menuItem;
        }

        [HttpPost("create-item")]
        [Authorize(Roles = "Admin")]  
        public async Task<ActionResult<MenuItem>> CreateMenuItem(MenuItem menuItem)
        {
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.Id }, menuItem);
        }

        [HttpPut("update-item/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] MenuItem menuItem)
        {
            var existingItem = await _context.MenuItems.FindAsync(id);
            if (existingItem == null)
                return NotFound(new { message = "Menu item not found." });

            // Update the existing item
            existingItem.CategoryId = menuItem.CategoryId;
            existingItem.Name = menuItem.Name;
            existingItem.Description = menuItem.Description;
            existingItem.CurrentPrice = menuItem.CurrentPrice;
            existingItem.IsAvailable = menuItem.IsAvailable;
            existingItem.UpdatedAt = DateTime.UtcNow; 

            await _context.SaveChangesAsync();
            return Ok(existingItem); 
        }


        [HttpDelete("delete-item/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound(new { message = "Menu item not found." });

            
            menuItem.IsAvailable = false;
            menuItem.UpdatedAt = DateTime.UtcNow; 

            await _context.SaveChangesAsync();
            return Ok(new { message = "Menu item marked as unavailable.", menuItem });
        }

    }
}
