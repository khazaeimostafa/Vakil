using FluentValidation;
using Met.DTOs;

namespace API.Fluent
{
    public class RegisterVMValidator : AbstractValidator<RegisterVM>
    {
        public RegisterVMValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotNull()
                .WithMessage("Email  نباس   is null");

            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithMessage("Email   نباس  is Empty");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("  lastName نباس is Empty");
        }
    }

    // public static class CustomValidators
    // {
    //     public static IRuleBuilderOptions<T, IList<TElement>>
    //     ListMustContainNumberOfItems<T, TElement>(
    //         this IRuleBuilder<T, IList<TElement>> ruleBuilder,
    //         int? min = null,
    //         int? max = null
    //     )
    //     {
             
    //     }
    }
 
