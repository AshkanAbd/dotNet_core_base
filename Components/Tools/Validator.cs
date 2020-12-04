using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using dotNet_base.Models;

namespace dotNet_base.Components.Tools
{
    public abstract class Validator<T> : AbstractValidator<T>
    {
        protected const string Default = "درخواست معتبر نیست.";

        protected Validator(BaseContext context = null)
        {
        }

        public ValidationResult StdValidate(ValidationContext<T> context)
        {
            return (ValidationResult) Validate(context);
        }

        public Task<ValidationResult> StdValidateAsync(ValidationContext<T> context,
            CancellationToken cancellation = new CancellationToken())
        {
            return ValidateAsync(context, cancellation)
                .ContinueWith(x => (ValidationResult) x.Result, cancellation);
        }


        public ValidationResult StdValidate(T instance)
        {
            return ValidationResult.FromFluentValidationResult(Validate(instance));
        }

        public Task<ValidationResult> StdValidateAsync(T instance,
            CancellationToken cancellation = new CancellationToken())
        {
            return ValidateAsync(instance, cancellation)
                .ContinueWith(x => ValidationResult.FromFluentValidationResult(x.Result), cancellation);
        }
    }
}