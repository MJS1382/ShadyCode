using System.ComponentModel.DataAnnotations;
using PersonalBlog.Models;

public class Role
{
    public int Id { get; set; }
    [Required] [StringLength(128)]
    public required string Name { get; set; }

    public List<ApplicationUser> Users { get; set; } = new();
}