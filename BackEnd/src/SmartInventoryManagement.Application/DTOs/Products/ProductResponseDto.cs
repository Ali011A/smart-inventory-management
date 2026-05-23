using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.DTOs.Products
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
          public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
            public string? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
