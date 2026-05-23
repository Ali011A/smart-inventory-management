using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Warehouses;
using SmartInventoryManagement.Application.Interfaces.Services;

namespace SmartInventoryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WarehouseResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _warehouseService.GetAllActiveAsync();
            return Ok(result);
        }

        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<WarehouseResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] QueryParameters parameters)
        {
            var result = await _warehouseService.GetPagedAsync(parameters);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(WarehouseResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _warehouseService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(WarehouseResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateWarehouseDto dto)
        {
            var result = await _warehouseService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(WarehouseResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateWarehouseDto dto)
        {
            var result = await _warehouseService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await _warehouseService.DeleteAsync(id);
            return NoContent();
        }
    }
}
