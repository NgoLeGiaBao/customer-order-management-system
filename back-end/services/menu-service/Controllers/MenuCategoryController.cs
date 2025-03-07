using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using menu_service.Data;
using menu_service.Models;
using Microsoft.EntityFrameworkCore;

namespace menu_service.Controllers
{
    [Route("menu-category")]
    [ApiController]
    public class MenuCategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MenuCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("get-all-categories")]
        public async Task<ActionResult<IEnumerable<MenuCategory>>> GetCategoriesWithItems()
        {
            var categories = await _context.MenuCategories
                .Include(c => c.MenuItems)
                .ToListAsync();

            return Ok(categories);
        }


        [HttpGet("get-category-by-id/{id}")]
        public async Task<ActionResult<MenuCategory>> GetCategory(int id)
        {
             var category = await _context.MenuCategories
                .Include(c => c.MenuItems) 
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost("create-category")]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<MenuCategory>> CreateCategory(MenuCategory category)
        {
            // Check if the category name already exists
            bool exists = await _context.MenuCategories.AnyAsync(c => c.Name == category.Name);
            if (exists)
            {
                return Conflict(new { status = 409, message = "Category name already exists" });
            }

            // Add the new category
            _context.MenuCategories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        [HttpPut("update-category/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, MenuCategory category)
        {
            var existingCategory = await _context.MenuCategories.FindAsync(id);
            if (existingCategory == null)
                return NotFound("Category not found");

            // Check if the new name already exists
            bool nameExists = await _context.MenuCategories
                .AnyAsync(c => c.Name == category.Name && c.Id != id);

            if (nameExists)
                return Conflict(new { message = "Category name already exists" });

            // Update the existing category
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            // Save changes
            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the category." });
            }
        }


        [HttpDelete("delete-category/{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.MenuCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Set category as unavailable instead of deleting
            category.IsAvailable = false;
        
            await _context.SaveChangesAsync();
            return Ok(new { message = "Menu item marked as unavailable.", category });
        }
    }
}
