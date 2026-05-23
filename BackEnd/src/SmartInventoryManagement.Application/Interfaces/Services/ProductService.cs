using AutoMapper;
using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Products;
using SmartInventoryManagement.Application.Interfaces.Repositories;
using SmartInventoryManagement.Domain.Entities;
using SmartInventoryManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.Interfaces.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductResponseDto>> GetAllAsync(
            QueryParameters parameters)
        {
            var result = await _productRepo.GetAllAsync(parameters);
            return new PagedResult<ProductResponseDto>
            {
                Items = _mapper.Map<IEnumerable<ProductResponseDto>>(result.Items),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<ProductResponseDto> GetByIdAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Product), id);
            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
        {
            if (await _productRepo.SkuExistsAsync(dto.SKU))
                throw new ConflictException(
                    $"A product with SKU '{dto.SKU}' already exists.");

            var product = _mapper.Map<Product>(dto);
            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _productRepo.GetTrackedByIdAsync(id)
                ?? throw new NotFoundException(nameof(Product), id);

            _mapper.Map(dto, product);
            _productRepo.Update(product);
            await _productRepo.SaveChangesAsync();

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepo.GetTrackedByIdAsync(id)
                ?? throw new NotFoundException(nameof(Product), id);

            product.IsDeleted = true;
            _productRepo.Update(product);
            await _productRepo.SaveChangesAsync();
        }
    }
}
