using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ExamPortal.Legacy.Web.Services
{
    /// <summary>Protects the report access codes that are emailed to candidates.</summary>
    public static class CryptoHelper
    {
        private static readonly byte[] Key = Encoding.ASCII.GetBytes("EP2014!k");
        private static readonly byte[] Iv = Encoding.ASCII.GetBytes("00000000");

        public static string Encrypt(string plainText)
        {
            using (var des = new DESCryptoServiceProvider())
            using (var stream = new MemoryStream())
            {
                using (var crypto = new CryptoStream(stream, des.CreateEncryptor(Key, Iv), CryptoStreamMode.Write))
                {
                    var bytes = Encoding.UTF8.GetBytes(plainText);
                    crypto.Write(bytes, 0, bytes.Length);
                }
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static string Decrypt(string cipherText)
        {
            using (var des = new DESCryptoServiceProvider())
            using (var stream = new MemoryStream())
            {
                using (var crypto = new CryptoStream(stream, des.CreateDecryptor(Key, Iv), CryptoStreamMode.Write))
                {
                    var bytes = Convert.FromBase64String(cipherText);
                    crypto.Write(bytes, 0, bytes.Length);
                }
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
