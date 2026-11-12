using ExamPortal.Legacy.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExamPortal.Legacy.Tests
{
    [TestClass]
    public class CryptoHelperTests
    {
        [TestMethod]
        public void Encrypt_ThenDecrypt_ReturnsOriginalText()
        {
            var cipherText = CryptoHelper.Encrypt("1|maria.alvarez@example.com");

            Assert.AreEqual("1|maria.alvarez@example.com", CryptoHelper.Decrypt(cipherText));
        }

        [TestMethod]
        public void Encrypt_DoesNotReturnThePlainText()
        {
            Assert.IsFalse(CryptoHelper.Encrypt("maria.alvarez@example.com").Contains("maria"));
        }
    }
}
