using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly IHttpContextAccessor _httpContextAccessor;


        public TableService(ApplicationDbContext context, IMessageBus messageBus, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _messageBus = messageBus;
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
            var table = await _context.Tables.FirstOrDefaultAsync(t => t.TableNumber == tableNumber);
            if (table == null) throw new Exception($"Table {tableNumber} not found.");
            if (table.Status == TableStatus.Occupied) throw new Exception($"Table {tableNumber} is already occupied.");

            table.Status = TableStatus.Occupied;
            await _context.SaveChangesAsync();

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
