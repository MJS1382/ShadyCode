

using System.ComponentModel.DataAnnotations;

namespace PersonalBlog.Validation;

public sealed class IntegerOnly : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (!int.TryParse((string?)value, out _))
        {
            return new ValidationResult($"{value} is not an integer");
        }

        return ValidationResult.Success;
    }
}