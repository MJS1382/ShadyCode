using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using PersonalBlog.Data;
using PersonalBlog.Models;

namespace PersonalBlog.Services.Auth;

public sealed class AuthService(
    ApplicationDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    IEmailService emailService,
    IEmailVerificationTokenService emailVerificationTokenService,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<RegisterResult> RegisterAsync(RegisterInputModel input)
    {
        if (input is null || input.Email is null || input.UserName is null || input.Password is null)
            return RegisterResult.InvalidInput;

        if(await dbContext.Users.AnyAsync(u => u.Email == input.Email.Trim().ToLowerInvariant() && u.RegistrationConfirmed))
            return RegisterResult.EmailAlreadyExists;

        if(await dbContext.Users.AnyAsync(u => u.UserName == input.UserName.Trim().ToLowerInvariant()))
            return RegisterResult.UserNameAlreadyExists;

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = input.UserName!.Trim().ToLowerInvariant(),
            Email = input.Email!.Trim().ToLowerInvariant(),
            RegisteredAt = DateTime.UtcNow,
            PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(input.Password, 12, BCrypt.Net.HashType.SHA256),
            IsOptInForNotifications = !input.IsOptInForNotifications,
            SecurityStamp = Guid.NewGuid()
        };

        // await dbContext.Users.AddAsync(user);
        // await dbContext.SaveChangesAsync();

        var token = emailVerificationTokenService.GenerateToken(user.Id);
        var encodedToken = System.Web.HttpUtility.UrlEncode(token);
        var confirmationLink = $"{httpContextAccessor.HttpContext!.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}/auth/confirm-email?username={input.UserName}&email={input.Email}&token={encodedToken}";

        try
        {
            bool emailResult = await emailService.SendEmailAsync(
                input.Email,
                "Email confirmation",
                $"""
                    <h2>Welcome to shady code</h2>
                    <h3>thanks for signing up to my website</h3>
                    <h3>Please confirm your email address {input.Email} to continue</h3>
                    <p>click the link bellow to confirm :</p>
                    <a href="{confirmationLink}">Confirm</a>
                """
            );

            if (!emailResult) return RegisterResult.FailedToSendEmail;
        }
        catch /*(Exception ex)*/
        {
            // Inject Ilogger and log the exception later
            return RegisterResult.FailedToSendEmail;
        }

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        return RegisterResult.Success;
    }

    async Task<bool> IAuthService.ConfirmAccountAsync(string? username, string? email, string? confirmationToken)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(confirmationToken))
        {
            logger.LogInformation("oh not some info was null or empty");
            return false;
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email.Trim().ToLowerInvariant());
        if (user == null) return false;

        if (emailVerificationTokenService.ValidateToken(confirmationToken, user.Id, out Guid verifiedUserId)
            && !user.RegistrationConfirmed)
        {
            user.RegistrationConfirmed = true;
            user.SecurityStamp = Guid.NewGuid();
            await dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<ApplicationUser?> AuthenticateAsync(string email, string password)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (user is null) return null;
        if (!user.RegistrationConfirmed) return null;
        
        var storedPasswordHash = user.PasswordHash;
        var verificationResult = BCrypt.Net.BCrypt.EnhancedVerify(password, storedPasswordHash, BCrypt.Net.HashType.SHA256);

        if (!verificationResult) return null;

        user.SecurityStamp = Guid.NewGuid();
        await dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<ApplicationUser?> GetCurrentUser(AuthenticationStateProvider authStateProvider)
    {
        var authState = await authStateProvider.GetAuthenticationStateAsync();
        var usernameClaim = authState.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        if (usernameClaim == null) return null;

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == usernameClaim!.Value);
        user!.PasswordHash = string.Empty;
        user!.Id = Guid.AllBitsSet;
        return user;
    }

    public Task<bool> IsAccountConfirmedAsync()
    {
        var isConfirmed = httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == "RegistrationConfirmed")?.Value == "true";
        return Task.FromResult(isConfirmed);
    }
}