namespace SmartInventoryManagement.API.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddAngularCors(
            this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
