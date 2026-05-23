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
    public class InventoryTransactionConfiguration
     : IEntityTypeConfiguration<InventoryTransaction>
    {
        public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
        {
            builder.HasKey(t => t.Id);

            // Store enum as integer — compact, indexed efficiently, stable
            builder.Property(t => t.Type)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(t => t.Quantity)
                .IsRequired();

            builder.Property(t => t.Notes)
                .HasMaxLength(500);

           
            builder.Property(t => t.UserId)
                .IsRequired()
                .HasMaxLength(450);

       
            builder.HasIndex(t => t.CreatedAt);

        
            builder.HasIndex(t => t.ProductId);

            builder.HasOne(t => t.Product)
                .WithMany(p => p.InventoryTransactions)
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(t => t.Warehouse)
                .WithMany(w => w.InventoryTransactions)
                .HasForeignKey(t => t.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction   );

        }
    }
}
