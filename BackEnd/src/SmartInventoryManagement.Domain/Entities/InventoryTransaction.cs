using SmartInventoryManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Domain.Entities
{
    public class InventoryTransaction
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public TransactionType Type { get; set; }

        public int Quantity { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
            public Product Product { get; set; } = null!;
            public Warehouse Warehouse { get; set; } = null!;
    }
}
