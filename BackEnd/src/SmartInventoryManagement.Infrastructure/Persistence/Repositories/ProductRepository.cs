using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Products;
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
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetTrackedByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PagedResult<Product>> GetAllAsync(QueryParameters parameters)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var term = parameters.SearchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.SKU.ToLower().Contains(term) ||
                    (p.Category != null && p.Category.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.Name)
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };
        }

        public async Task<bool> SkuExistsAsync(string sku, int? excludeId = null)
        {
            var query = _context.Products.Where(p => p.SKU == sku);
            if (excludeId.HasValue)
                query = query.Where(p => p.Id != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
