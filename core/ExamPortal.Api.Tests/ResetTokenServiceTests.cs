using ExamPortal.Api.Services;
using Xunit;

namespace ExamPortal.Api.Tests;

public class ResetTokenServiceTests
{
    [Fact]
    public void Redeem_WithIssuedToken_ReturnsTrue()
    {
        var service = new ResetTokenService();
        var token = service.Issue("alice");

        Assert.True(service.Redeem("alice", token));
    }

    [Fact]
    public void Redeem_WithTokenForAnotherUser_ReturnsFalse()
    {
        var service = new ResetTokenService();
        var token = service.Issue("alice");

        Assert.False(service.Redeem("bob", token));
    }
}
