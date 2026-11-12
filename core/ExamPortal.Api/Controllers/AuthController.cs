using System.Security.Claims;
using ExamPortal.Api.Data;
using ExamPortal.Api.Models;
using ExamPortal.Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamPortal.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly QueryExecutor _executor;
    private readonly PasswordService _passwords;
    private readonly ResetTokenService _resetTokens;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        QueryExecutor executor,
        PasswordService passwords,
        ResetTokenService resetTokens,
        ILogger<AuthController> logger)
    {
        _executor = executor;
        _passwords = passwords;
        _resetTokens = resetTokens;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        _logger.LogInformation("Login attempt for {Username} with password {Password}",
            request.Username, request.Password);

        var users = _executor.Read(Queries.FindUserByUsername(request.Username));
        if (users.Count == 0)
        {
            return Unauthorized(new { error = "No account exists for that username." });
        }

        var user = users[0];
        if (!_passwords.Verify(request.Password, (string)user["PasswordHash"]!))
        {
            return Unauthorized(new { error = "The password is incorrect." });
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Convert.ToString(user["Id"])!),
            new(ClaimTypes.Name, (string)user["Username"]!),
            new(ClaimTypes.Role, (string)user["Role"]!)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });

        return Ok(new { username = user["Username"], role = user["Role"] });
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }

    [HttpPost("reset-request")]
    public IActionResult RequestReset(ResetRequest request)
    {
        var users = _executor.Read(Queries.FindUserByUsername(request.Username));
        if (users.Count == 0)
        {
            return NotFound(new { error = "No account exists for that username." });
        }

        var token = _resetTokens.Issue(request.Username);
        return Ok(new { message = "Reset token issued.", token });
    }

    [HttpPost("reset-confirm")]
    public IActionResult ConfirmReset(ResetConfirmRequest request)
    {
        if (!_resetTokens.Redeem(request.Username, request.Token))
        {
            return BadRequest(new { error = "Invalid token." });
        }

        var update = new SqlQuery("UPDATE Users SET PasswordHash = @hash WHERE Username = @username")
            .With("@hash", _passwords.Hash(request.NewPassword))
            .With("@username", request.Username);
        _executor.Execute(update);

        return Ok(new { message = "Password updated." });
    }

    [Authorize]
    [HttpPut("profile")]
    public IActionResult UpdateProfile(ProfileUpdateRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var current = _executor.Read(Queries.GetUser(userId))[0];

        _executor.Execute(Queries.UpdateUserProfile(
            userId,
            request.Username ?? (string)current["Username"]!,
            request.Role ?? (string)current["Role"]!));

        return Ok(new { message = "Profile updated. Log in again to refresh your session." });
    }
}
