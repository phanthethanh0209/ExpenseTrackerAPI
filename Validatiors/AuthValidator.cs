using ExpenseTrackerAPI.DTOs;
using FluentValidation;

namespace ExpenseTrackerAPI.Validatiors
{
    public class AuthValidator : AbstractValidator<RegisterDTO>
    {
        public AuthValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,30}$").WithMessage("Password must be between 8 and 30 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required");

        }

    }
}
