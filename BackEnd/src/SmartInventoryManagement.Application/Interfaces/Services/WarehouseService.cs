using AutoMapper;
using SmartInventoryManagement.Application.Common;
using SmartInventoryManagement.Application.DTOs.Warehouses;
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
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _repository;
        private readonly IMapper _mapper;

        public WarehouseService(IWarehouseRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WarehouseResponseDto>> GetAllActiveAsync()
        {
            var warehouses = await _repository.GetAllActiveAsync();
            return _mapper.Map<IEnumerable<WarehouseResponseDto>>(warehouses);
        }

        public async Task<PagedResult<WarehouseResponseDto>> GetPagedAsync(
            QueryParameters parameters)
        {
            var result = await _repository.GetPagedAsync(parameters);
            return new PagedResult<WarehouseResponseDto>
            {
                Items = _mapper.Map<IEnumerable<WarehouseResponseDto>>(result.Items),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<WarehouseResponseDto> GetByIdAsync(int id)
        {
            var warehouse = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Warehouse), id);
            return _mapper.Map<WarehouseResponseDto>(warehouse);
        }

        public async Task<WarehouseResponseDto> CreateAsync(CreateWarehouseDto dto)
        {
            if (await _repository.NameExistsAsync(dto.Name))
                throw new ConflictException($"A warehouse named '{dto.Name}' already exists.");

            var warehouse = _mapper.Map<Warehouse>(dto);
            await _repository.AddAsync(warehouse);
            await _repository.SaveChangesAsync();

            return _mapper.Map<WarehouseResponseDto>(warehouse);
        }

        public async Task<WarehouseResponseDto> UpdateAsync(int id, UpdateWarehouseDto dto)
        {
            var warehouse = await _repository.GetTrackedByIdAsync(id)
                ?? throw new NotFoundException(nameof(Warehouse), id);

            if (await _repository.NameExistsAsync(dto.Name, id))
                throw new ConflictException($"A warehouse named '{dto.Name}' already exists.");

            _mapper.Map(dto, warehouse);
            _repository.Update(warehouse);
            await _repository.SaveChangesAsync();

            return _mapper.Map<WarehouseResponseDto>(warehouse);
        }

        public async Task DeleteAsync(int id)
        {
            var warehouse = await _repository.GetTrackedByIdAsync(id)
                ?? throw new NotFoundException(nameof(Warehouse), id);

            if (await _repository.HasActiveStockAsync(id))
                throw new BusinessException(
                    "Cannot delete a warehouse with active stock. " +
                    "Relocate or clear all inventory first.");

            warehouse.IsDeleted = true;
            _repository.Update(warehouse);
            await _repository.SaveChangesAsync();
        }
    }
}
