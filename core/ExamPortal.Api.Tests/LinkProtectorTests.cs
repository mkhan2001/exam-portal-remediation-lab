using ExamPortal.Api.Services;
using Xunit;

namespace ExamPortal.Api.Tests;

public class LinkProtectorTests
{
    [Fact]
    public void Protect_ThenUnprotect_ReturnsOriginalValue()
    {
        var protector = new LinkProtector();

        var token = protector.Protect("registration:42");

        Assert.Equal("registration:42", protector.Unprotect(token));
    }

    [Fact]
    public void Protect_DoesNotReturnThePlainValue()
    {
        var protector = new LinkProtector();

        Assert.DoesNotContain("registration", protector.Protect("registration:42"));
    }
}
