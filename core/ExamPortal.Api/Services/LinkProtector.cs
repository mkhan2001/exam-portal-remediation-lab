using System.Security.Cryptography;
using System.Text;

namespace ExamPortal.Api.Services;

/// <summary>Creates the opaque tokens used in score report download links.</summary>
public class LinkProtector
{
    private static readonly byte[] Key = Encoding.ASCII.GetBytes("ExamPortalKey123");

    public string Protect(string value)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        var cipher = aes.EncryptEcb(Encoding.UTF8.GetBytes(value), PaddingMode.PKCS7);
        return Convert.ToBase64String(cipher);
    }

    public string Unprotect(string token)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        var plain = aes.DecryptEcb(Convert.FromBase64String(token), PaddingMode.PKCS7);
        return Encoding.UTF8.GetString(plain);
    }
}
