using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.DTOs.Inventory
{
    public class TransactionResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty; // "In" or "Out"
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public string PerformedBy { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; }
    }
}
