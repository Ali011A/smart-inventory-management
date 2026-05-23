using SmartInventoryManagement.API.Extensions;
using SmartInventoryManagement.API.Middleware;
using SmartInventoryManagement.Infrastructure;
using SmartInventoryManagement.Infrastructure.Identity;
using SmartInventoryManagement.Application;
var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerWithJwt();

builder.Services.AddAngularCors();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await AdminSeeder.SeedAsync(scope.ServiceProvider);
}

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


