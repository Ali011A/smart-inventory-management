using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Warehouses;
using SmartInventoryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.Interfaces.Repositories
{
    public interface IWarehouseRepository
    {
        Task<Warehouse?> GetByIdAsync(int id);
        Task<Warehouse?> GetTrackedByIdAsync(int id);
        Task<IEnumerable<Warehouse>> GetAllActiveAsync();
        Task<PagedResult<Warehouse>> GetPagedAsync(QueryParameters parameters);
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
        Task<bool> HasActiveStockAsync(int warehouseId);
        Task AddAsync(Warehouse warehouse);
        void Update(Warehouse warehouse);
        Task SaveChangesAsync();
    }
}
