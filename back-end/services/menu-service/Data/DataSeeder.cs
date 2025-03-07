using menu_service.Models;
using Microsoft.EntityFrameworkCore;

namespace menu_service.Data
{
    public static class DataSeeder
    {
        public static void SeedData(ApplicationDbContext context)
        {
            if (!context.MenuCategories.Any())
            {
                var categories = new List<MenuCategory>
                {
                    new MenuCategory { Name = "Hotpot", Description = "Various types of hotpot dishes" },
                    new MenuCategory { Name = "Grilled", Description = "Delicious grilled food" },
                    new MenuCategory { Name = "Fast Food", Description = "Quick and tasty meals" },
                    new MenuCategory { Name = "Seafood", Description = "Fresh seafood dishes" },
                    new MenuCategory { Name = "Desserts", Description = "Sweet treats and desserts" },
                    new MenuCategory { Name = "Drinks", Description = "Refreshing beverages" }
                };

                context.MenuCategories.AddRange(categories);
                context.SaveChanges();
            }

            if (!context.MenuItems.Any())
            {
                var menuItems = new List<MenuItem>
                {
                    // Hotpot
                    new MenuItem { CategoryId = 1, Name = "Spicy Sichuan Hotpot", Description = "Spicy and flavorful hotpot with Sichuan peppers", CurrentPrice = 350_000 },
                    new MenuItem { CategoryId = 1, Name = "Tom Yum Hotpot", Description = "Thai-style hotpot with a sour and spicy broth", CurrentPrice = 320_000 },
                    new MenuItem { CategoryId = 1, Name = "Beef Mushroom Hotpot", Description = "A mild and savory hotpot with fresh mushrooms and beef", CurrentPrice = 330_000 },

                    // Grilled
                    new MenuItem { CategoryId = 2, Name = "Grilled Pork Ribs", Description = "Juicy pork ribs marinated with special BBQ sauce", CurrentPrice = 280_000 },
                    new MenuItem { CategoryId = 2, Name = "Korean BBQ Beef", Description = "Tender beef slices grilled with Korean spices", CurrentPrice = 310_000 },
                    new MenuItem { CategoryId = 2, Name = "Grilled Chicken Wings", Description = "Crispy grilled chicken wings with honey glaze", CurrentPrice = 220_000 },

                    // Fast Food
                    new MenuItem { CategoryId = 3, Name = "Cheeseburger", Description = "Beef patty with melted cheese and fresh vegetables", CurrentPrice = 120_000 },
                    new MenuItem { CategoryId = 3, Name = "Fried Chicken Combo", Description = "Crispy fried chicken served with fries", CurrentPrice = 140_000 },
                    new MenuItem { CategoryId = 3, Name = "Pepperoni Pizza", Description = "Classic pizza topped with pepperoni and mozzarella", CurrentPrice = 180_000 },

                    // Seafood
                    new MenuItem { CategoryId = 4, Name = "Grilled Lobster", Description = "Freshly grilled lobster served with butter sauce", CurrentPrice = 500_000 },
                    new MenuItem { CategoryId = 4, Name = "Garlic Butter Prawns", Description = "Juicy prawns sautéed in garlic butter sauce", CurrentPrice = 350_000 },
                    new MenuItem { CategoryId = 4, Name = "Salt and Pepper Squid", Description = "Crispy deep-fried squid seasoned with salt and pepper", CurrentPrice = 280_000 },

                    // Desserts
                    new MenuItem { CategoryId = 5, Name = "Chocolate Lava Cake", Description = "Warm chocolate cake with a molten center", CurrentPrice = 90_000 },
                    new MenuItem { CategoryId = 5, Name = "Mango Sticky Rice", Description = "Thai dessert with sweet mango and coconut sticky rice", CurrentPrice = 85_000 },
                    new MenuItem { CategoryId = 5, Name = "Matcha Cheesecake", Description = "Creamy cheesecake with a hint of matcha flavor", CurrentPrice = 95_000 },

                    // Drinks
                    new MenuItem { CategoryId = 6, Name = "Vietnamese Iced Coffee", Description = "Strong coffee with sweetened condensed milk", CurrentPrice = 45_000 },
                    new MenuItem { CategoryId = 6, Name = "Fresh Watermelon Juice", Description = "Refreshing juice made from fresh watermelon", CurrentPrice = 50_000 },
                    new MenuItem { CategoryId = 6, Name = "Milk Tea with Tapioca", Description = "Classic milk tea with chewy tapioca pearls", CurrentPrice = 60_000 }
                };

                context.MenuItems.AddRange(menuItems);
                context.SaveChanges();
            }
        }
    }
}
