namespace PersonalBlog.Services;

public interface IEmailService 
{
    Task<bool> SendEmailAsync(string emailAddress, string subject, string htmlString);
}