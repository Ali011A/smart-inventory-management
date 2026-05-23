using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Inventory;
using SmartInventoryManagement.Application.Interfaces.Services;

namespace SmartInventoryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpPost("transactions")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ProcessTransaction(
            [FromBody] TransactionRequestDto dto)
        {
            await _inventoryService.ProcessTransactionAsync(dto);
            return NoContent();
        }

        [HttpGet("history")]
        [ProducesResponseType(typeof(PagedResult<TransactionResponseDto>),
            StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistory(
            [FromQuery] QueryParameters parameters,
            [FromQuery] int? productId = null,
            [FromQuery] int? warehouseId = null)
        {
            var result = await _inventoryService
                .GetHistoryAsync(parameters, productId, warehouseId);
            return Ok(result);
        }

        [HttpGet("stock")]
        [ProducesResponseType(typeof(IEnumerable<InventoryItemResponseDto>),
            StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentStock(
            [FromQuery] int? warehouseId = null)
        {
            var result = await _inventoryService.GetCurrentStockAsync(warehouseId);
            return Ok(result);
        }
    }
}
