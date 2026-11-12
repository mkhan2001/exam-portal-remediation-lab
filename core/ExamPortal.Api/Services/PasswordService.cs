using System.Security.Cryptography;
using System.Text;

namespace ExamPortal.Api.Services;

public class PasswordService
{
    public string Hash(string password)
    {
        var bytes = SHA1.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public bool Verify(string password, string storedHash)
    {
        return Hash(password) == storedHash;
    }
}
