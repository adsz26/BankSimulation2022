using BankSimulation.Models.EF;
using FluentValidation;

namespace BankSimulation
{
    public class CustomerValidator : AbstractValidator<Customer>
    {
        // Ez a validáció van használatban

        public CustomerValidator()
        {
            RuleFor(customer => customer.CustomerName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Név mező nem lehet üres.")
                .Length(3, 50).WithMessage("Név mező nem lehet kisebb 3 és nem lehet nagyobb 50 karakter hosszúságnál.");

            RuleFor(customer => customer.CustomerPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Jelszó mező nem lehet üres.")
                .Length(3, 50).WithMessage("Jelszó mező nem lehet kisebb 3 és nem lehet nagyobb 50 karakter hosszúságnál.");

            RuleFor(customer => customer.CustomerAccountNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Számlaszám mező nem lehet üres.")
                .Matches("^[0-9]{8}-[0-9]{8}(-[0-9]{8})?$").WithMessage("Számlaszám mező formátuma nem megfelelő.\n\tPélda jó bankszámlaszámra: 12345678-12345678");

            RuleFor(customer => customer.CustomerCreditAccount)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Hitelszámla mező nem lehet üres.")
                .Matches("^[0-9]{8}-[0-9]{8}(-[0-9]{8})?$").WithMessage("Hitelszámla mező formátuma nem megfelelő.\n\tPélda jó hitelszámlaszámra: 12345678-12345678");

        }

        public bool IsValidBalance(string input)
        {
            if (!float.TryParse(input, out _))
            {
                return false;
            }
            else
            {
                return false;
            }
        }

    }
}
