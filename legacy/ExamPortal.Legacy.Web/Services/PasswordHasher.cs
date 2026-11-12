using System.Security.Cryptography;
using System.Text;

namespace ExamPortal.Legacy.Web.Services
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool Verify(string password, string storedHash)
        {
            return Hash(password) == storedHash;
        }
    }
}
