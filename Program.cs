using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PersonalBlog.Data;
using PersonalBlog.Services;
using PersonalBlog.Components;
using PersonalBlog.Services.Auth;
using PersonalBlog.Services.Redis;
using StackExchange.Redis;
using PersonalBlog.Middlewares;
using PersonalBlog.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddValidation();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.LoginPath = "/auth/login";
    options.LogoutPath = "/auth/logout";
    options.AccessDeniedPath = "/access-denied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    //AI Assisted Code Begins
    options.Events.OnValidatePrincipal = async context =>
    {
        var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var securityStampClaim = context.Principal?.FindFirst("SecurityStamp")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId)
            || !Guid.TryParse(securityStampClaim, out var securityStamp))
        {
            await RejectInvalidCookieAsync(context);
            return;
        }

        var dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
        var userState = await dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => new { user.SecurityStamp, user.RegistrationConfirmed })
            .FirstOrDefaultAsync(context.HttpContext.RequestAborted);

        if (userState is null
            || !userState.RegistrationConfirmed
            || userState.SecurityStamp != securityStamp)
        {
            await RejectInvalidCookieAsync(context);
        }
    };
    //AI Assisted Code Ends
});
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization(
    options => {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    }
);

builder.Services.AddHttpContextAccessor();

builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddScoped<IEmailVerificationTokenService, EmailVerificationTokenService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Redis";
});
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    var connectionString = builder.Configuration.GetConnectionString("Redis")
        ?? throw new InvalidOperationException("Redis connection string is missing.");

    var options = ConfigurationOptions.Parse(connectionString);
    options.AbortOnConnectFail = false;

    return ConnectionMultiplexer.Connect(options);
});
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
builder.Services.AddSingleton<IHyperLogLogStore, HyperLogLogStore>();

builder.Services.AddHostedService<CacheToDbService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    //manually applying migrations
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseVisitorIdSpecification();

app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGroup("/api/blog")
    .MapBlogEndpoints();
    //.RequireAuthorization();

app.Run();

//AI Assisted Code Begins
static async Task RejectInvalidCookieAsync(CookieValidatePrincipalContext context)
{
    context.RejectPrincipal();
    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
}
//AI Assisted Code Ends