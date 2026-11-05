namespace PersonalBlog.Middlewares;

public sealed class VisitorIdSpecificationMiddleware(RequestDelegate next)
{
    private const string CookieName = "visitor_id";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Cookies.TryGetValue(CookieName, out var visitorId) || !Guid.TryParse(visitorId, out _))
        {
            visitorId = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions
            {
                IsEssential = false,
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddYears(1),
            };
            context.Response.Cookies.Append(CookieName, visitorId, cookieOptions);
        }
        context.Items[CookieName] = visitorId;

        await next(context);
    }
}

public static class VisitorIdSpecificationMiddlewareExtensions
{
    public static IApplicationBuilder UseVisitorIdSpecification(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<VisitorIdSpecificationMiddleware>();
    }
}