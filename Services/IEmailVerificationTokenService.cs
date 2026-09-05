namespace PersonalBlog.Services;

public interface IEmailVerificationTokenService
{
    string GenerateToken(Guid userId);
    bool ValidateToken(string token, Guid userId, out Guid verifiedUserId);
}