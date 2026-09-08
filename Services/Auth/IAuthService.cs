using Microsoft.AspNetCore.Components.Authorization;
using PersonalBlog.Models;

namespace PersonalBlog.Services.Auth;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterInputModel input);
    Task<ApplicationUser?> AuthenticateAsync(string email, string password);
    Task<bool> ConfirmAccountAsync(string? username, string? email, string? confirmationToken);
    Task<bool> IsAccountConfirmedAsync();
    Task<ApplicationUser?> GetCurrentUser(AuthenticationStateProvider authStateProvider);

    // Task<ApplicationUser?> GetUserByEmailAsync(string email);
    // Task<ApplicationUser?> GetUserByIdAsync(Guid userId);
    //Task<bool> IsUserInRoleAsync(ApplicationUser user, string roleName);
    //Task<List<string>> GetUserRolesAsync(ApplicationUser user);
}