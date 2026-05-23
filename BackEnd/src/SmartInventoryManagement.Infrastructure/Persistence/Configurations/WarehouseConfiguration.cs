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
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(w => w.Location)
                .HasMaxLength(500);

            builder.HasQueryFilter(w => !w.IsDeleted);
        }
    }
}
