using Microsoft.Extensions.DependencyInjection;
using SmartInventoryManagement.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
namespace SmartInventoryManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

           
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IWarehouseService, WarehouseService>();
            services.AddScoped<IInventoryService, InventoryService>();

            return services;
        }
    }
}
