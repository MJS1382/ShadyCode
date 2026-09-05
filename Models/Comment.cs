using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PersonalBlog.Models;

public sealed class Comment
{
    public Guid Id { get; set; }
    public Guid? ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    public ApplicationUser User { get; set; } = null!;
    public Guid PostId { get; set; }
    [Required] [StringLength(512)]
    public required string Text { get; set; }
    public int Likes { get; set; }
    [Required]
    public DateTime Date { get; set; }
}