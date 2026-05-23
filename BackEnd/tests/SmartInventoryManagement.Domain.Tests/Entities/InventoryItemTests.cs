using FluentAssertions;
using SmartInventoryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Domain.Tests.Entities
{
    public class InventoryItemTests
    {
        [Fact]
        public void InventoryItem_QuantityDefaultsToZero_WhenCreated()
        {
            // Arrange + Act
            var item = new InventoryItem
            {
                ProductId = 1,
                WarehouseId = 1
            };

            // Assert
            item.Quantity.Should().Be(0);
        }

        [Fact]
        public void InventoryItem_CanHoldPositiveQuantity()
        {
            // Arrange + Act
            var item = new InventoryItem
            {
                ProductId = 1,
                WarehouseId = 1,
                Quantity = 100
            };

            // Assert
            item.Quantity.Should().Be(100);
        }

        [Fact]
        public void InventoryItem_AllowsZeroQuantity_AfterFullStockOut()
        {
            // Arrange
            var item = new InventoryItem
            {
                ProductId = 1,
                WarehouseId = 1,
                Quantity = 10
            };

            // Act
            item.Quantity -= 10;

            // Assert
            item.Quantity.Should().Be(0);
        }
    }
}
