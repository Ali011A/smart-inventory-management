using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Products;
using SmartInventoryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetTrackedByIdAsync(int id);
        Task<PagedResult<Product>> GetAllAsync(QueryParameters parameters);
        Task<bool> SkuExistsAsync(string sku, int? excludeId = null);
        Task AddAsync(Product product);
        void Update(Product product);
        Task SaveChangesAsync();


    }
}
