using System.ComponentModel.DataAnnotations;

namespace PersonalBlog.Models;

public sealed class CommentInputModel
{
    [Required]
    public Guid PostId { get; set; }
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Content { get; set; }

}