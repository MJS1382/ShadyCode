using System.ComponentModel.DataAnnotations;

namespace PersonalBlog.Validation
{
    public class PasswordValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string password)
            {
                return new ValidationResult("Password must be a string.");
            }

            if (password.Length < 8)
            {
                return new ValidationResult("Password must be at least 8 characters long.");
            }

            if (!HasUpperCase(password))
            {
                return new ValidationResult("Password must contain at least one uppercase letter.");
            }

            if (!HasLowerCase(password))
            {
                return new ValidationResult("Password must contain at least one lowercase letter.");
            }

            if (!HasDigit(password))
            {
                return new ValidationResult("Password must contain at least one digit.");
            }

            if (!HasSpecialCharacter(password))
            {
                return new ValidationResult("Password must contain at least one special character.");
            }

            return ValidationResult.Success;
        }

        private bool HasUpperCase(string password) => password.Any(char.IsUpper);
        private bool HasLowerCase(string password) => password.Any(char.IsLower);
        private bool HasDigit(string password) => password.Any(char.IsDigit);
        private bool HasSpecialCharacter(string password) => password.Any(ch => !char.IsLetterOrDigit(ch));
    }
}