using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Domain.Entities
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Product Product { get; set; } = null!; //Ef core will automatically populate
            public Warehouse Warehouse { get; set; } = null!;
    }
}
