using System.ComponentModel.DataAnnotations;
using PersonalBlog.Validation;

namespace PersonalBlog.Models;

public sealed class EditorInputModel
{
    public Guid Id { get; set; }
    [Required] [StringLength(64)]
    public string? Title { get; set; }

    [Required] [StringLength(128)]
    public string? Slug { get; set; }

    [StringLength(64)]
    public string? Tags { get; set; }

    [Required(ErrorMessage = "The estimated time field is required.")] [IntegerOnly]
    public string? ReadTimeSeconds { get; set; }

    [StringLength(128)]
    public string? ThumbnailPath { get; set; }
}