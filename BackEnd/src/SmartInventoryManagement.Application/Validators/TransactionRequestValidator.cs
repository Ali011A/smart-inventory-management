using FluentValidation;
using SmartInventoryManagement.Application.DTOs.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Application.Validators
{
    public class TransactionRequestValidator:AbstractValidator<TransactionRequestDto>
    {
        public TransactionRequestValidator()
        {
            RuleFor(x => x.ProductId)
             .GreaterThan(0).WithMessage("Valid ProductId is required.");

            RuleFor(x => x.WarehouseId)
                .GreaterThan(0).WithMessage("Valid WarehouseId is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            RuleFor(x => x.Note)
                .MaximumLength(500).When(x => x.Note != null);


        }
    }
}
