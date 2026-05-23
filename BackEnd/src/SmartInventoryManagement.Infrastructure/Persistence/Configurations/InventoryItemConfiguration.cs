using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventoryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Infrastructure.Persistence.Configurations
{
    public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Quantity)
                .IsRequired();

        
            builder.HasIndex(i => new { i.ProductId, i.WarehouseId })
                .IsUnique();

            builder.HasOne(i => i.Product)
                .WithMany(p => p.InventoryItems)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
 

            builder.HasOne(i => i.Warehouse)
                .WithMany(w => w.InventoryItems)
                .HasForeignKey(i => i.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
