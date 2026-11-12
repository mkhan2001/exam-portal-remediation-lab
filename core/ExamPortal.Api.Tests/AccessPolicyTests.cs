using System.Security.Claims;
using ExamPortal.Api.Services;
using Xunit;

namespace ExamPortal.Api.Tests;

public class AccessPolicyTests
{
    private static ClaimsPrincipal UserWith(int id, string role)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Role, role)
        }, "test");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public void CanManage_Owner_ReturnsTrue()
    {
        Assert.True(new AccessPolicy().CanManage(UserWith(1, "Scheduler"), ownerUserId: 1));
    }

    [Fact]
    public void CanManage_AnotherScheduler_ReturnsFalse()
    {
        Assert.False(new AccessPolicy().CanManage(UserWith(2, "Scheduler"), ownerUserId: 1));
    }

    [Fact]
    public void CanManage_Admin_ReturnsTrue()
    {
        Assert.True(new AccessPolicy().CanManage(UserWith(3, "Admin"), ownerUserId: 1));
    }
}
