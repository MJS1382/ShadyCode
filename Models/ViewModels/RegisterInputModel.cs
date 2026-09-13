using System.ComponentModel.DataAnnotations;
using PersonalBlog.Validation;

namespace PersonalBlog.Models;

public sealed class RegisterInputModel
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(64, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 64 characters.")]
    [RegularExpression(
        @"^[a-zA-Z0-9](?:[a-zA-Z0-9_]*[a-zA-Z0-9])?$",
        ErrorMessage = "Username can only contain letters, numbers, and underscores in the middle."
    )]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [StringLength(64, ErrorMessage = "Email cannot exceed 64 characters.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(64, ErrorMessage = "Password cannot exceed 64 characters.")]
    [PasswordValidation]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Confirm Password is required.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string? ConfirmPassword { get; set; }
    
    public bool IsOptInForNotifications { get; set; }
}

public enum RegisterResult
{
    Success,
    UserNameAlreadyExists,
    EmailAlreadyExists,
    InvalidInput,
    FailedToSendEmail
}