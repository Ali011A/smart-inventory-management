using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.Interfaces.Services
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseResponseDto>> GetAllActiveAsync();
        Task<PagedResult<WarehouseResponseDto>> GetPagedAsync(QueryParameters parameters);
        Task<WarehouseResponseDto> GetByIdAsync(int id);
        Task<WarehouseResponseDto> CreateAsync(CreateWarehouseDto dto);
        Task<WarehouseResponseDto> UpdateAsync(int id, UpdateWarehouseDto dto);
        Task DeleteAsync(int id);
    }
}
