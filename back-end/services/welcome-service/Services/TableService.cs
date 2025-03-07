using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using WelcomeService.Data;
using WelcomeService.Enums;
using WelcomeService.Models;
using WelcomeService.RabbitMQ;

namespace WelcomeService.Services
{
    public class TableService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMessageBus _messageBus;
        private readonly IConnectionMultiplexer _redis;

        private readonly IHttpContextAccessor _httpContextAccessor;


        public TableService(ApplicationDbContext context, IMessageBus messageBus, IConnectionMultiplexer connectionMultiplexer, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _messageBus = messageBus;
            _redis = connectionMultiplexer;
            _httpContextAccessor = httpContextAccessor;
        }
 

        public async Task<List<Table>> GetAllTables()
        {
            return await _context.Tables.ToListAsync();
        }

        public async Task<Table> GetTableByNumber(int tableNumber)
        {
            return await _context.Tables.FirstOrDefaultAsync(t => t.TableNumber == tableNumber);
        }

        public async Task<Table> OpenTable(int tableNumber)
        {
            // Get Redis database
            var redisDb = _redis.GetDatabase();
            var tableStatusKey = $"table:{tableNumber}:status";

            // Check if table is already occupied
            var statusInRedis = await redisDb.StringGetAsync(tableStatusKey);
            if (!statusInRedis.IsNullOrEmpty && statusInRedis == TableStatus.Occupied.ToString())
            {
                throw new Exception($"Table {tableNumber} is already occupied.");
            }


            var table = await _context.Tables.FirstOrDefaultAsync(t => t.TableNumber == tableNumber);
            if (table == null) 
                throw new Exception($"Table {tableNumber} not found.");
            
            if (table.Status == TableStatus.Occupied) 
                throw new Exception($"Table {tableNumber} is already occupied.");

            table.Status = TableStatus.Occupied;
            await _context.SaveChangesAsync();

            // Set table status in Redis
            await redisDb.StringSetAsync(tableStatusKey, TableStatus.Occupied.ToString(), TimeSpan.FromMinutes(2));

            // Get employee ID from token
            var employeeId = GetEmployeeIdFromToken();

            // Send event to RabbitMQ
            _messageBus.Publish("table.opened", new { TableNumber = tableNumber, EmployeeId = employeeId});

            return table;
        }

        public async Task<Table> CloseTable(int tableNumber)
        {
            var table = await _context.Tables.FirstOrDefaultAsync(t => t.TableNumber == tableNumber);
            if (table == null) throw new Exception($"Table {tableNumber} not found.");
            if (table.Status == TableStatus.Available) throw new Exception($"Table {tableNumber} is already available.");

            table.Status = TableStatus.Available;
            await _context.SaveChangesAsync();

            // Remove the key from Redis (if exists)
            await _redis.GetDatabase().KeyDeleteAsync($"table:{tableNumber}:status");

            // Send event to RabbitMQ
            _messageBus.Publish("table.closed", new { TableNumber = tableNumber, Status = "Closed", Timestamp = DateTime.UtcNow });

            return table;
        }

        // Get employee ID from token
        private int GetEmployeeIdFromToken()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) 
                throw new UnauthorizedAccessException("User not authenticated");

            var employeeIdClaim = user.Claims.FirstOrDefault(c => c.Type == "EmployeeId");
            if (employeeIdClaim == null) 
                throw new UnauthorizedAccessException("EmployeeId claim not found");

            return int.Parse(employeeIdClaim.Value);
        }
    
    }
}
