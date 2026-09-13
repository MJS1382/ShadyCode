using System.ComponentModel.DataAnnotations;

namespace PersonalBlog.Models;

public sealed class AuthenticationInputModel
{
    [Required(ErrorMessage = "You must enter your email address")]
    [EmailAddress(ErrorMessage = "Not a valid email address")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "You must enter your password")]
    public string? Password { get; set; }
    public bool RememberMe { get; set; }
}

public enum AuthenticationResult
{
    Success,
    InvalidCredentials,
    UserNotFound,
    EmailNotConfirmed
}