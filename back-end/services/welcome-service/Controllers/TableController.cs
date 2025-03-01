using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WelcomeService.Services;
using WelcomeService.RabbitMQ;

namespace WelcomeService.Controllers
{
    [Route("api/tables")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly TableService _tableService;
        private readonly IMessageBus _messageBus;

        public TableController(TableService tableService, IMessageBus messageBus)
        {
            _tableService = tableService;
            _messageBus = messageBus;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrWaiterPolicy")]
        public async Task<IActionResult> GetTables()
        {
            var tables = await _tableService.GetAllTables();
            return Ok(tables);
        }

        [HttpGet("{tableNumber}")]
        [Authorize(Policy = "AdminOrWaiterPolicy")]
        public async Task<IActionResult> GetTable(int tableNumber)
        {
            var table = await _tableService.GetTableByNumber(tableNumber);
            if (table == null) return NotFound(new { message = $"Table {tableNumber} not found." });

            return Ok(table);
        }

        [HttpPost("{tableNumber}/open")]
        [Authorize(Policy = "AdminOrWaiterPolicy")]
        public async Task<IActionResult> OpenTable(int tableNumber)
        {
            try
            {
                var table = await _tableService.OpenTable(tableNumber);
                return Ok(new { message = $"Table {tableNumber} opened successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{tableNumber}/close")]
        [Authorize(Policy = "AdminOrWaiterPolicy")]
        public async Task<IActionResult> CloseTable(int tableNumber)
        {
            try
            {
                var table = await _tableService.CloseTable(tableNumber);
                return Ok(new { message = $"Table {tableNumber} closed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
