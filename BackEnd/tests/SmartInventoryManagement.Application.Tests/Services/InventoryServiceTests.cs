using AutoMapper;
using FluentAssertions;
using Moq;
using SmartInventoryManagement.Application.DTOs.Inventory;
using SmartInventoryManagement.Application.Interfaces.Repositories;
using SmartInventoryManagement.Application.Interfaces.Services;
using SmartInventoryManagement.Domain.Entities;
using SmartInventoryManagement.Domain.Enums;
using SmartInventoryManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.Tests.Services
{
    public class InventoryServiceTests
    {
        private readonly Mock<IInventoryRepository> _inventoryRepoMock;
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<IWarehouseRepository> _warehouseRepoMock;
        private readonly Mock<ICurrentUserService> _currentUserMock;
        
        private readonly InventoryService _sut;

        public InventoryServiceTests()
        {
            _inventoryRepoMock = new Mock<IInventoryRepository>();
            _productRepoMock = new Mock<IProductRepository>();
            _warehouseRepoMock = new Mock<IWarehouseRepository>();
            _currentUserMock = new Mock<ICurrentUserService>();
            

            _currentUserMock.Setup(x => x.UserId).Returns("test-user-id");

            _sut = new InventoryService(
                _inventoryRepoMock.Object,
                _productRepoMock.Object,
                _warehouseRepoMock.Object,
                _currentUserMock.Object
                
            );
        }

    

        [Fact]
        public async Task ProcessTransactionAsync_StockIn_CreatesNewInventoryItem_WhenNoneExists()
        {
            // Arrange
            var dto = new TransactionRequestDto
            {
                ProductId = 1,
                WarehouseId = 1,
                Type = TransactionType.In,
                Quantity = 10
            };

            _productRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Product { Id = 1, Name = "Test" });

            _warehouseRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Warehouse { Id = 1, Name = "Main" });

            _inventoryRepoMock
                .Setup(r => r.GetByProductAndWarehouseAsync(1, 1))
                .ReturnsAsync((InventoryItem?)null);

            // Act
            await _sut.ProcessTransactionAsync(dto);

            // Assert
            _inventoryRepoMock.Verify(r => r.AddItemAsync(
                It.Is<InventoryItem>(i =>
                    i.ProductId == 1 &&
                    i.WarehouseId == 1 &&
                    i.Quantity == 10)),
                Times.Once);

            _inventoryRepoMock.Verify(r => r.AddTransactionAsync(
                It.Is<InventoryTransaction>(t =>
                    t.Type == TransactionType.In &&
                    t.Quantity == 10 &&
                    t.UserId == "test-user-id")),
                Times.Once);

            _inventoryRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessTransactionAsync_StockIn_IncreasesExistingQuantity()
        {
            // Arrange
            var existingItem = new InventoryItem
            {
                Id = 1,
                ProductId = 1,
                WarehouseId = 1,
                Quantity = 20
            };

            var dto = new TransactionRequestDto
            {
                ProductId = 1,
                WarehouseId = 1,
                Type = TransactionType.In,
                Quantity = 5
            };

            _productRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Product { Id = 1 });

            _warehouseRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Warehouse { Id = 1 });

            _inventoryRepoMock
                .Setup(r => r.GetByProductAndWarehouseAsync(1, 1))
                .ReturnsAsync(existingItem);

            // Act
            await _sut.ProcessTransactionAsync(dto);

            // Assert
            existingItem.Quantity.Should().Be(25);
            _inventoryRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        
        [Fact]
        public async Task ProcessTransactionAsync_StockOut_DecreasesQuantity_WhenSufficientStock()
        {
            // Arrange
            var existingItem = new InventoryItem
            {
                Id = 1,
                ProductId = 1,
                WarehouseId = 1,
                Quantity = 50
            };

            var dto = new TransactionRequestDto
            {
                ProductId = 1,
                WarehouseId = 1,
                Type = TransactionType.Out,
                Quantity = 20
            };

            _productRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Product { Id = 1 });

            _warehouseRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Warehouse { Id = 1 });

            _inventoryRepoMock
                .Setup(r => r.GetByProductAndWarehouseAsync(1, 1))
                .ReturnsAsync(existingItem);

            // Act
            await _sut.ProcessTransactionAsync(dto);

            // Assert
            existingItem.Quantity.Should().Be(30);
            _inventoryRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessTransactionAsync_StockOut_ThrowsBusinessException_WhenInsufficientStock()
        {
            // Arrange
            var existingItem = new InventoryItem
            {
                Id = 1,
                ProductId = 1,
                WarehouseId = 1,
                Quantity = 5
            };

            var dto = new TransactionRequestDto
            {
                ProductId = 1,
                WarehouseId = 1,
                Type = TransactionType.Out,
                Quantity = 10
            };

            _productRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Product { Id = 1 });

            _warehouseRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Warehouse { Id = 1 });

            _inventoryRepoMock
                .Setup(r => r.GetByProductAndWarehouseAsync(1, 1))
                .ReturnsAsync(existingItem);

            // Act
            var act = async () => await _sut.ProcessTransactionAsync(dto);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("*Insufficient stock*");

            _inventoryRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ProcessTransactionAsync_StockOut_ThrowsBusinessException_WhenNoInventoryExists()
        {
            // Arrange
            var dto = new TransactionRequestDto
            {
                ProductId = 1,
                WarehouseId = 1,
                Type = TransactionType.Out,
                Quantity = 5
            };

            _productRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Product { Id = 1 });

            _warehouseRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Warehouse { Id = 1 });

            _inventoryRepoMock
                .Setup(r => r.GetByProductAndWarehouseAsync(1, 1))
                .ReturnsAsync((InventoryItem?)null);

            // Act
            var act = async () => await _sut.ProcessTransactionAsync(dto);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("*stock OUT*");

            _inventoryRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

     

        [Fact]
        public async Task ProcessTransactionAsync_ThrowsNotFoundException_WhenProductNotFound()
        {
            // Arrange
            var dto = new TransactionRequestDto
            {
                ProductId = 99,
                WarehouseId = 1,
                Type = TransactionType.In,
                Quantity = 5
            };

            _productRepoMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Product?)null);

            // Act
            var act = async () => await _sut.ProcessTransactionAsync(dto);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*99*");
        }

        [Fact]
        public async Task ProcessTransactionAsync_ThrowsNotFoundException_WhenWarehouseNotFound()
        {
            // Arrange
            var dto = new TransactionRequestDto
            {
                ProductId = 1,
                WarehouseId = 99,
                Type = TransactionType.In,
                Quantity = 5
            };

            _productRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Product { Id = 1 });

            _warehouseRepoMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Warehouse?)null);

            // Act
            var act = async () => await _sut.ProcessTransactionAsync(dto);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*99*");
        }

        
        [Fact]
        public async Task ProcessTransactionAsync_CallsSaveChangesExactlyOnce_ForValidTransaction()
        {
            // Arrange
            var existingItem = new InventoryItem
            {
                ProductId = 1,
                WarehouseId = 1,
                Quantity = 100
            };

            var dto = new TransactionRequestDto
            {
                ProductId = 1,
                WarehouseId = 1,
                Type = TransactionType.Out,
                Quantity = 10
            };

            _productRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Product { Id = 1 });

            _warehouseRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Warehouse { Id = 1 });

            _inventoryRepoMock
                .Setup(r => r.GetByProductAndWarehouseAsync(1, 1))
                .ReturnsAsync(existingItem);

            // Act
            await _sut.ProcessTransactionAsync(dto);

            // Assert 
            _inventoryRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
