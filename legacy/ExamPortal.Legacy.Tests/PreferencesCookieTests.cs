using ExamPortal.Legacy.Web.Models;
using ExamPortal.Legacy.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExamPortal.Legacy.Tests
{
    [TestClass]
    public class PreferencesCookieTests
    {
        [TestMethod]
        public void Deserialize_WithEmptyCookie_ReturnsDefaults()
        {
            var preferences = PreferencesCookie.Deserialize(null);

            Assert.AreEqual("light", preferences.Theme);
            Assert.AreEqual(25, preferences.PageSize);
        }

        [TestMethod]
        public void Serialize_ThenDeserialize_ReturnsSameValues()
        {
            var original = new UserPreferences { Theme = "dark", PageSize = 50 };

            var roundTripped = PreferencesCookie.Deserialize(PreferencesCookie.Serialize(original));

            Assert.AreEqual("dark", roundTripped.Theme);
            Assert.AreEqual(50, roundTripped.PageSize);
        }
    }
}
