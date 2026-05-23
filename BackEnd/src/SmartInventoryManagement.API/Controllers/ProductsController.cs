using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Products;
using SmartInventoryManagement.Application.Interfaces.Services;

namespace SmartInventoryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ProductResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] QueryParameters parameters)
        {
            var result = await _productService.GetAllAsync(parameters);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var result = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            var result = await _productService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}
