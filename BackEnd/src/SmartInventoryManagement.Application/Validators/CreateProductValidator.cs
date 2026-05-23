using FluentValidation;
using SmartInventoryManagement.Application.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.Validators
{
    public class CreateProductValidator: AbstractValidator<CreateProductDto>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");
            RuleFor(x => x.SKU)
                        .NotEmpty().WithMessage("SKU is required.")
                        .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters.")
                        .Matches(@"^[A-Z0-9\-]+$")
                        .WithMessage("SKU must contain only uppercase letters, numbers, and hyphens.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).When(x => x.Description != null);
        }
    }
}
