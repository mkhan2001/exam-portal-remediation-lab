using ExamPortal.Api.Data;
using ExamPortal.Api.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<Db>();
builder.Services.AddSingleton<QueryExecutor>();
builder.Services.AddSingleton<PasswordService>();
builder.Services.AddSingleton<ResetTokenService>();
builder.Services.AddSingleton<LinkProtector>();
builder.Services.AddSingleton<ArchiveService>();
builder.Services.AddSingleton<AccessPolicy>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "examportal_auth";
        options.Cookie.HttpOnly = false;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.Cookie.SameSite = SameSiteMode.None;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .SetIsOriginAllowed(_ => true)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

app.Services.GetRequiredService<Db>().Initialize();

app.UseDeveloperExceptionPage();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "ExamPortal API is running. See README.md for the endpoints.");

app.Run();

public partial class Program { }
