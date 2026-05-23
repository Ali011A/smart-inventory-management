using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Warehouses;
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
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly AppDbContext _context;

        public WarehouseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Warehouse?> GetByIdAsync(int id)
        {
            return await _context.Warehouses
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Warehouse?> GetTrackedByIdAsync(int id)
        {
            return await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<IEnumerable<Warehouse>> GetAllActiveAsync()
        {
            return await _context.Warehouses
                .AsNoTracking()
                .OrderBy(w => w.Name)
                .ToListAsync();
        }

        public async Task<PagedResult<Warehouse>> GetPagedAsync(QueryParameters parameters)
        {
            var query = _context.Warehouses.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var term = parameters.SearchTerm.Trim().ToLower();
                query = query.Where(w =>
                    w.Name.ToLower().Contains(term) ||
                    (w.Location != null && w.Location.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(w => w.Name)
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<Warehouse>
            {
                Items = items,
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            var query = _context.Warehouses.Where(w => w.Name == name);
            if (excludeId.HasValue)
                query = query.Where(w => w.Id != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task<bool> HasActiveStockAsync(int warehouseId)
        {
            return await _context.InventoryItems
                .AnyAsync(i => i.WarehouseId == warehouseId && i.Quantity > 0);
        }

        public async Task AddAsync(Warehouse warehouse)
        {
            await _context.Warehouses.AddAsync(warehouse);
        }

        public void Update(Warehouse warehouse)
        {
            _context.Warehouses.Update(warehouse);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
