using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Inventory;
using SmartInventoryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.Interfaces.Repositories
{
    public interface IInventoryRepository
    {
        Task<InventoryItem?> GetByProductAndWarehouseAsync(
       int productId, int warehouseId);
        Task<PagedResult<TransactionResponseDto>> GetHistoryAsync(
            QueryParameters parameters, int? productId = null, int? warehouseId = null);
        Task<IEnumerable<InventoryItemResponseDto>> GetCurrentStockAsync(
            int? warehouseId = null);
        Task AddItemAsync(InventoryItem item);
        Task AddTransactionAsync(InventoryTransaction transaction);
        Task SaveChangesAsync();
    }
}
