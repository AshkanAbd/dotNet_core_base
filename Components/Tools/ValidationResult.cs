using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace dotNet_base.Components.Tools
{
    public class ValidationResult : FluentValidation.Results.ValidationResult
    {
        public ValidationResult()
        {
        }

        public ValidationResult(IEnumerable<ValidationFailure> failures) : base(failures)
        {
        }

        public static ValidationResult FromFluentValidationResult(FluentValidation.Results.ValidationResult result)
        {
            return new ValidationResult(result.Errors);
        }

        public IEnumerable Messages()
        {
            return Errors.Select(x => new {
                x.PropertyName,
                x.ErrorMessage,
            });
        }
    }
}