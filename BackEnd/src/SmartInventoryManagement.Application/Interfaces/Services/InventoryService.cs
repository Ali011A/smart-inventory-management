using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Inventory;
using SmartInventoryManagement.Application.Interfaces.Repositories;
using SmartInventoryManagement.Domain.Entities;
using SmartInventoryManagement.Domain.Enums;
using SmartInventoryManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.Interfaces.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IProductRepository _productRepo;
        private readonly IWarehouseRepository _warehouseRepo;
        private readonly ICurrentUserService _currentUser;

        public InventoryService(
            IInventoryRepository inventoryRepo,
            IProductRepository productRepo,
            IWarehouseRepository warehouseRepo,
            ICurrentUserService currentUser)
        {
            _inventoryRepo = inventoryRepo;
            _productRepo = productRepo;
            _warehouseRepo = warehouseRepo;
            _currentUser = currentUser;
        }

        public async Task ProcessTransactionAsync(TransactionRequestDto dto)
        {
            
            _ = await _productRepo.GetByIdAsync(dto.ProductId)
                ?? throw new NotFoundException(nameof(Product), dto.ProductId);

            _ = await _warehouseRepo.GetByIdAsync(dto.WarehouseId)
                ?? throw new NotFoundException(nameof(Warehouse), dto.WarehouseId);

            var inventoryItem = await _inventoryRepo
                .GetByProductAndWarehouseAsync(dto.ProductId, dto.WarehouseId);

            if (inventoryItem == null)
            {
                if (dto.Type == TransactionType.Out)
                    throw new BusinessException(
                        "Cannot perform stock OUT: " +
                        "no inventory record exists for this product in this warehouse.");

                
                // Create the record with zero quantity — the addition below will set it.
                inventoryItem = new InventoryItem
                {
                    ProductId = dto.ProductId,
                    WarehouseId = dto.WarehouseId,
                    Quantity = 0,
                    UpdatedAt = DateTime.UtcNow
                };
                await _inventoryRepo.AddItemAsync(inventoryItem);
            }

            if (dto.Type == TransactionType.Out)
            {
                
                // Stock cannot go negative 
                // Check BEFORE modifying any state.
                if (inventoryItem.Quantity < dto.Quantity)
                    throw new BusinessException(
                        $"Insufficient stock. " +
                        $"Requested: {dto.Quantity}, Available: {inventoryItem.Quantity}.");

                inventoryItem.Quantity -= dto.Quantity;
            }
            else
            {
                inventoryItem.Quantity += dto.Quantity;
            }

            inventoryItem.UpdatedAt = DateTime.UtcNow;


            var transaction = new InventoryTransaction
            {
                ProductId = dto.ProductId,
                WarehouseId = dto.WarehouseId,
                UserId = _currentUser.UserId,
                Type = dto.Type,
                Quantity = dto.Quantity,
                Notes = dto.Note,
                CreatedAt = DateTime.UtcNow
            };

            await _inventoryRepo.AddTransactionAsync(transaction);

          
            await _inventoryRepo.SaveChangesAsync();
        }

        public async Task<PagedResult<TransactionResponseDto>> GetHistoryAsync(
            QueryParameters parameters, int? productId = null, int? warehouseId = null)
        {
            return await _inventoryRepo.GetHistoryAsync(parameters, productId, warehouseId);
        }

        public async Task<IEnumerable<InventoryItemResponseDto>> GetCurrentStockAsync(
            int? warehouseId = null)
        {
            return await _inventoryRepo.GetCurrentStockAsync(warehouseId);
        }
    }
}
