using AutoMapper;
using FluentAssertions;
using Moq;
using SmartInventoryManagement.Application.DTOs.Products;
using SmartInventoryManagement.Application.Interfaces.Repositories;
using SmartInventoryManagement.Application.Interfaces.Services;
using SmartInventoryManagement.Domain.Entities;
using SmartInventoryManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
namespace SmartInventoryManagement.Application.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductService _sut;

        public ProductServiceTests()
        {
            _repoMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _sut = new ProductService(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenProductExists()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Laptop", SKU = "LAP-001" };
            var dto = new ProductResponseDto { Id = 1, Name = "Laptop", SKU = "LAP-001" };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductResponseDto>(product)).Returns(dto);

            // Act
            var result = await _sut.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.SKU.Should().Be("LAP-001");
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsNotFoundException_WhenProductNotFound()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

            // Act
            var act = async () => await _sut.GetByIdAsync(99);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*99*");
        }

        [Fact]
        public async Task CreateAsync_ThrowsConflictException_WhenSkuExists()
        {
            // Arrange
            var dto = new CreateProductDto { Name = "Laptop", SKU = "LAP-001", Price = 999 };
            _repoMock.Setup(r => r.SkuExistsAsync("LAP-001", null)).ReturnsAsync(true);

            // Act
            var act = async () => await _sut.CreateAsync(dto);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("*LAP-001*");

            _repoMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_AddsProductAndSaves_WhenSkuIsUnique()
        {
            // Arrange
            var dto = new CreateProductDto { Name = "Laptop", SKU = "LAP-001", Price = 999 };
            var product = new Product { Id = 1, Name = "Laptop", SKU = "LAP-001" };
            var responseDto = new ProductResponseDto { Id = 1, Name = "Laptop", SKU = "LAP-001" };

            _repoMock.Setup(r => r.SkuExistsAsync("LAP-001", null)).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<Product>(dto)).Returns(product);
            _mapperMock.Setup(m => m.Map<ProductResponseDto>(product)).Returns(responseDto);

            // Act
            var result = await _sut.CreateAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.SKU.Should().Be("LAP-001");
            _repoMock.Verify(r => r.AddAsync(product), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsNotFoundException_WhenProductNotFound()
        {
            // Arrange
            _repoMock.Setup(r => r.GetTrackedByIdAsync(99)).ReturnsAsync((Product?)null);

            // Act
            var act = async () => await _sut.DeleteAsync(99);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_SoftDeletesProduct_WhenFound()
        {
            // Arrange
            var product = new Product { Id = 1, IsDeleted = false };
            _repoMock.Setup(r => r.GetTrackedByIdAsync(1)).ReturnsAsync(product);

            // Act
            await _sut.DeleteAsync(1);

            // Assert
            product.IsDeleted.Should().BeTrue();
            _repoMock.Verify(r => r.Update(product), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
