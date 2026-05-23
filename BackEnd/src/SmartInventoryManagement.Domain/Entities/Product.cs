using SmartInventoryManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Domain.Entities
{
    public class Product: BaseEntity
    {
        public string Name { get; set; } =string.Empty;
        public string SKU { get; set; } =string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
      public string Category { get; set; } =string.Empty;

        public ICollection<InventoryItem> InventoryItems { get; set; }
            = new List<InventoryItem>();
            public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
            = new List<InventoryTransaction>();

    }
}
