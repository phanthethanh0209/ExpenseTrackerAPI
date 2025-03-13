using ExpenseTrackerAPI.DTOs;
using FluentValidation;

namespace ExpenseTrackerAPI.Validatiors
{
    public class ExpenseValidator : AbstractValidator<ExpenseRequestDTO>
    {
        public ExpenseValidator()
        {
            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Amount is required")
                .GreaterThan(0).WithMessage("Amount must be greater than 0");

            RuleFor(x => x.CategoryID)
                .NotEmpty().WithMessage("Category is required");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Date cannot be greater than current date");

        }
    }
}
