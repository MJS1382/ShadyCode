using System.ComponentModel.DataAnnotations;

public class ProjectMetadata
{
    public Guid Id { get; set; }
    public Guid? PostId { get; set; }
    public Post Post { get; set; } = null!;
    [Required] [StringLength(128)]
    public string ProjectName { get; set; } = null!;
    [StringLength(512)]
    public string GithubUrl { get; set; } = null!;
    [StringLength(512)]
    public string DemoUrl { get; set; } = null!;
    [StringLength(512)]
    public string DownloadUrl { get; set; } = null!;
    [StringLength(256)]
    public string Technologies { get; set; } = null!;
    public ProjectStatus Status { get; set; }
}

public enum ProjectStatus
{
    InProgress,
    Completed,
    Archived,
    Canceled
}