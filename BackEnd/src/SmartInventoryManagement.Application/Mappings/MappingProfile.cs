using AutoMapper;
using SmartInventoryManagement.Application.DTOs.Inventory;
using SmartInventoryManagement.Application.DTOs.Products;
using SmartInventoryManagement.Application.DTOs.Warehouses;
using SmartInventoryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product
            CreateMap<Product, ProductResponseDto>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.SKU, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            // Warehouse
            CreateMap<Warehouse, WarehouseResponseDto>();
            CreateMap<CreateWarehouseDto, Warehouse>();
            CreateMap<UpdateWarehouseDto, Warehouse>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            // Inventory
            CreateMap<InventoryItem, InventoryItemResponseDto>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.SKU,
                    opt => opt.MapFrom(src => src.Product.SKU))
                .ForMember(dest => dest.WarehouseName,
                    opt => opt.MapFrom(src => src.Warehouse.Name))
                .ForMember(dest => dest.LastUpdated,
                    opt => opt.MapFrom(src => src.UpdatedAt));
        }
    }
}
