using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.DTOs.Warehouses
{
    public class WarehouseResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
