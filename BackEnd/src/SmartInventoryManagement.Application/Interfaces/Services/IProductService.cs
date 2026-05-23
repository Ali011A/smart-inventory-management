using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductResponseDto>> GetAllAsync(QueryParameters parameters);
        Task<ProductResponseDto> GetByIdAsync(int id);
        Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
        Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto);
        Task DeleteAsync(int id);

    }
}
