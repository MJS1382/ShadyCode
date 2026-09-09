using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PersonalBlog.Models;

namespace PersonalBlog.Models;

public sealed class Post
{
    public Guid Id { get; set; }
    public Guid? ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    public ApplicationUser Author { get; set; } = null!;
    [Required] [StringLength(128)]
    public required string Title { get; set; }
    [StringLength(256)]
    public string Slug { get; set; } = string.Empty;
    [StringLength(256)]
    public string Tags { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    [StringLength(512)]
    public string ThumbnailPath { get; set; } = string.Empty;
    public int Likes { get; set; }
    public int ViewCount { get; set; }
    public int ReadTimeSeconds { get; set; }
    public DateTime PublishDate { get; set; }
    public List<Comment> Comments { get; set; } = new();
    public ProjectMetadata? ProjectMetadata { get; set; }
}