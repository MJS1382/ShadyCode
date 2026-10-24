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
        if (string.IsNullOrWhiteSpace(input) || length <= 0)
            return string.Empty;

        var preview = System.Net.WebUtility.HtmlDecode(
            System.Text.RegularExpressions.Regex.Replace(input, "<[^>]*>", " "));
        preview = System.Text.RegularExpressions.Regex.Replace(preview, @"\s+", " ").Trim();

        if (preview.Length <= length)
            return preview;

        return preview[..length].TrimEnd() + "...";
    }
}