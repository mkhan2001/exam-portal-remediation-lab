using ExamPortal.Api.Services;
using Xunit;

namespace ExamPortal.Api.Tests;

public class PasswordServiceTests
{
    [Fact]
    public void Verify_WithCorrectPassword_ReturnsTrue()
    {
        var service = new PasswordService();
        var hash = service.Hash("Passw0rd!");

        Assert.True(service.Verify("Passw0rd!", hash));
    }

    [Fact]
    public void Verify_WithWrongPassword_ReturnsFalse()
    {
        var service = new PasswordService();
        var hash = service.Hash("Passw0rd!");

        Assert.False(service.Verify("passw0rd!", hash));
    }

    [Fact]
    public void Hash_DoesNotReturnThePlaintextPassword()
    {
        var service = new PasswordService();

        Assert.NotEqual("Passw0rd!", service.Hash("Passw0rd!"));
    }
}
