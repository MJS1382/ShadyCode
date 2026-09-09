using System.ComponentModel.DataAnnotations;

namespace PersonalBlog.Models;

public class ApplicationUser
{
    public Guid Id { get; set; }
    [Required] [StringLength(128)]
    public required string UserName { get; set; }
    [Required] [StringLength(128)] [EmailAddress]
    public required string Email { get; set; }
    [Required] [StringLength(256)]
    public required string PasswordHash { get; set; }
    public bool IsOptInForNotifications { get; set; }
    public bool RegistrationConfirmed { get; set; }
    public List<Role> Roles { get; set; } = new(); //nav property
    public List<Post> Posts { get; set; } = new();
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public Guid SecurityStamp { get; set; }
}