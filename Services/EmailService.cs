using System.Net.Mail;

namespace PersonalBlog.Services;

public sealed class EmailService(IConfiguration configuration, ILogger<IEmailService> logger) : IEmailService
{
    public async Task<bool> SendEmailAsync(string emailAddress, string subject, string htmlString)
    {
        var HostEmail = configuration["Email:Address"];
        var HostPass = configuration["Email:Password"];

        if (
            string.IsNullOrEmpty(emailAddress) ||
            string.IsNullOrEmpty(subject) || 
            string.IsNullOrEmpty(htmlString) ||
            string.IsNullOrEmpty(HostEmail)
        )
        {
            return await Task.FromResult(false);
        }

        MailMessage message = new MailMessage
        {
            Body = htmlString,
            From = new MailAddress(HostEmail!),
            Subject = subject,
            IsBodyHtml = true,
        };
        message.To.Add(emailAddress);

        using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Credentials = new System.Net.NetworkCredential(HostEmail, HostPass),
        };

        try
        {
            await Task.Run(() => smtpClient.Send(message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex.InnerException!.Message);
            return await Task.FromResult(false);
        }

        return await Task.FromResult(true);
    }
}