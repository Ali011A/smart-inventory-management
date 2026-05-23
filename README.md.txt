# Smart Inventory Management System

A full-stack inventory management system built using ASP.NET Core 8 Web API and Angular.

The system supports product management, warehouse management, inventory tracking, and stock transactions with JWT authentication and role-based authorization.

---

# Features

## Authentication
- JWT Authentication
- Role-based authorization
- Admin / Employee roles
- Secure login system

## Products
- Create product
- Update product
- Delete product (soft delete)
- View products list
- SKU uniqueness validation

## Warehouses
- Create warehouse
- Update warehouse
- Delete warehouse
- View warehouses list

## Inventory
- Stock IN transactions
- Stock OUT transactions
- Inventory history
- Negative stock prevention
- Inventory tracking per warehouse

## Additional Features
- Global Exception Middleware
- Pagination
- Validation
- Swagger/OpenAPI
- Unit Tests
- Clean Architecture

---

# Tech Stack

## Backend
- ASP.NET Core 8 Web API
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- JWT Authentication

## Frontend
- Angular
- Angular Material
- Reactive Forms
- HTTP Interceptors

## Testing
- xUnit
- Moq
- FluentAssertions

---
Backend Setup
1) Open backend API project
cmd--> backend/src/SmartInventoryManagement.API
2) Initialize User Secrets
dotnet user-secrets init
3) Configure Secrets

dotnet user-secrets set "Jwt:Secret" "YourSuperSecretKeyMinimum32CharactersLong"

dotnet user-secrets set "Jwt:Issuer" "SmartInventoryManagement"

dotnet user-secrets set "Jwt:Audience" "SmartInventoryManagement"

dotnet user-secrets set "Seed:AdminEmail" "admin@inventory.com"

dotnet user-secrets set "Seed:AdminPassword" "Admin@12345"
4) Apply Database Migrations
dotnet ef database update --project ../SmartInventoryManagement.Infrastructure --startup-project .
5) Run Backend API
dotnet run

Backend URL:

https://localhost:7265

Swagger URL:

https://localhost:7265/swagger
Frontend Setup
1) Open Angular project
cd frontend/smart-inventory-frontend
2) Install dependencies
npm install
3) Configure API URL

Open:

src/environments/environment.ts

Update:

export const environment = {
  production: false,
  apiUrl: 'https://localhost:7265/api'
};
4) Run Angular
ng serve

Frontend URL:

http://localhost:4200
Default Admin Account
Email	Password
admin@inventory.com	Admin@12345