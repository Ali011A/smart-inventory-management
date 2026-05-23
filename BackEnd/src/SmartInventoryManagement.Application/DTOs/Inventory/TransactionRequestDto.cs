using SmartInventoryManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.DTOs.Inventory
{
    public class TransactionRequestDto
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public TransactionType Type { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }
}
