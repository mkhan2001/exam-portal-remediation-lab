using ExamPortal.Legacy.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExamPortal.Legacy.Tests
{
    [TestClass]
    public class PasswordHasherTests
    {
        [TestMethod]
        public void Verify_WithCorrectPassword_ReturnsTrue()
        {
            var hash = PasswordHasher.Hash("Passw0rd!");

            Assert.IsTrue(PasswordHasher.Verify("Passw0rd!", hash));
        }

        [TestMethod]
        public void Verify_WithWrongPassword_ReturnsFalse()
        {
            var hash = PasswordHasher.Hash("Passw0rd!");

            Assert.IsFalse(PasswordHasher.Verify("passw0rd!", hash));
        }
    }
}
