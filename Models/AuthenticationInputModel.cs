namespace PersonalBlog.Models;

public sealed class AuthenticationInputModel
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public enum AuthenticationResult
{
    Success,
    InvalidCredentials,
    UserNotFound,
    EmailNotConfirmed
}