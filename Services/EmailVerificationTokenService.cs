using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

namespace PersonalBlog.Services;

public class EmailVerificationTokenService : IEmailVerificationTokenService
{
    private readonly ITimeLimitedDataProtector _protector;
    private readonly ILogger<EmailVerificationTokenService> _logger;

    public EmailVerificationTokenService(IDataProtectionProvider provider, ILogger<EmailVerificationTokenService> logger)
    {
        _protector = provider
            .CreateProtector("EmailVerification")
            .ToTimeLimitedDataProtector();

        _logger = logger;
    }

    public string GenerateToken(Guid userId)
    {
        return _protector.Protect(
            userId.ToString(),
            lifetime: TimeSpan.FromHours(24)
        );
    }

    public bool ValidateToken(string token, Guid userId, out Guid verifiedUserId)
    {
        try
        {
            verifiedUserId = Guid.Parse(_protector.Unprotect(token));

            if (verifiedUserId != userId)
                return false;
                
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Holy shit : {ex.Message}");
            verifiedUserId = Guid.Empty;
            return false;
        }
    }
}