using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.DTOs.Warehouses
{
    public class CreateWarehouseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
    }
}
