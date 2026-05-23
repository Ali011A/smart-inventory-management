using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.Interfaces.Services
{
    public interface IInventoryService
    {
        Task ProcessTransactionAsync(TransactionRequestDto dto);
        Task<PagedResult<TransactionResponseDto>> GetHistoryAsync(
            QueryParameters parameters, int? productId = null, int? warehouseId = null);
        Task<IEnumerable<InventoryItemResponseDto>> GetCurrentStockAsync(
            int? warehouseId = null);
    }
}
