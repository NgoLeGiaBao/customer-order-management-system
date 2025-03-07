using Microsoft.EntityFrameworkCore;
using WelcomeService.Enums;
using WelcomeService.Models;

namespace WelcomeService.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.Migrate();

            if (!context.Tables.Any())
            {
                context.Tables.AddRange(new List<Table>
                {
                    new Table { TableNumber = 1, Status = TableStatus.Available, Capacity = 4 },
                    new Table { TableNumber = 2, Status = TableStatus.Available, Capacity = 2 },
                    new Table { TableNumber = 3, Status = TableStatus.Available, Capacity = 4 }
                });

                context.SaveChanges();
            }
        }
    }
}
