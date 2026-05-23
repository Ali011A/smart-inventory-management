using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Inventory;
using SmartInventoryManagement.Application.Interfaces.Repositories;
using SmartInventoryManagement.Domain.Entities;
using SmartInventoryManagement.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Infrastructure.Persistence.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<InventoryItem?> GetByProductAndWarehouseAsync(
            int productId, int warehouseId)
        {
            return await _context.InventoryItems
                .FirstOrDefaultAsync(i =>
                    i.ProductId == productId &&
                    i.WarehouseId == warehouseId);
        }

        public async Task<IEnumerable<InventoryItemResponseDto>> GetCurrentStockAsync(
            int? warehouseId = null)
        {
            var query = _context.InventoryItems
                .AsNoTracking()
                .Include(i => i.Product)
                .Include(i => i.Warehouse)
                .AsQueryable();

            if (warehouseId.HasValue)
                query = query.Where(i => i.WarehouseId == warehouseId.Value);

            var items = await query.ToListAsync();

            return items.Select(i => new InventoryItemResponseDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                SKU = i.Product.SKU,
                WarehouseId = i.WarehouseId,
                WarehouseName = i.Warehouse.Name,
                Quantity = i.Quantity,
                LastUpdated = i.UpdatedAt
            });
        }

        public async Task<PagedResult<TransactionResponseDto>> GetHistoryAsync(
            QueryParameters parameters,
            int? productId = null,
            int? warehouseId = null)
        {
            var query = _context.InventoryTransactions
                .AsNoTracking()
                .Include(t => t.Product)
                .Include(t => t.Warehouse)
                .Join(
                    _context.Users,
                    t => t.UserId,
                    u => u.Id,
                    (t, u) => new { Transaction = t, UserEmail = u.Email }
                )
                .AsQueryable();

            if (productId.HasValue)
                query = query.Where(x => x.Transaction.ProductId == productId.Value);

            if (warehouseId.HasValue)
                query = query.Where(x => x.Transaction.WarehouseId == warehouseId.Value);

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var term = parameters.SearchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.Transaction.Product.Name.ToLower().Contains(term) ||
                    x.Transaction.Warehouse.Name.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Transaction.CreatedAt)
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var dtos = items.Select(x => new TransactionResponseDto
            {
                Id = x.Transaction.Id,
                ProductId = x.Transaction.ProductId,
                ProductName = x.Transaction.Product.Name,
                WarehouseId = x.Transaction.WarehouseId,
                WarehouseName = x.Transaction.Warehouse.Name,
                TransactionType = x.Transaction.Type.ToString(),
                Quantity = x.Transaction.Quantity,
                Note = x.Transaction.Notes  ,
                PerformedBy = x.UserEmail ?? "Unknown",
                CreatedAt = x.Transaction.CreatedAt
            });

            return new PagedResult<TransactionResponseDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };
        }

        public async Task AddItemAsync(InventoryItem item)
        {
            await _context.InventoryItems.AddAsync(item);
        }

        public async Task AddTransactionAsync(InventoryTransaction transaction)
        {
            await _context.InventoryTransactions.AddAsync(transaction);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
