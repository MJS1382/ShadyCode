namespace PersonalBlog.Models;

public class PostCardItem
{
    public string? Title { get; set; }
    public string? Contetnt { get; set; }
    public string? Slug { get; set; }
    public string[]? Tags { get; set; }
    public string? ThumbnailPath { get; set; }
    public int ViewCount { get; set; }
    public int ReadTimeSeconds { get; set; }
    public DateTime PublishDate { get; set; }

    public static string[] SetTags(string input) => input.Split(';');
    public static string? PreviewContent(string input, int length)
    {
        length = Math.Min(input.Length - 1, length);
        input = input[..length];
        int lastSpace = input.IndexOf("<br>");

        if (lastSpace > 0)
            input = input[..lastSpace];

        return input + "...";
    }
}