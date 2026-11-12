using ExamPortal.Legacy.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExamPortal.Legacy.Tests
{
    [TestClass]
    public class RosterImporterTests
    {
        [TestMethod]
        public void Parse_WithTwoCandidates_ReturnsBoth()
        {
            var xml =
                "<roster>" +
                "<candidate><firstName>Maria</firstName><lastName>Alvarez</lastName><email>maria@example.com</email></candidate>" +
                "<candidate><firstName>Devon</firstName><lastName>Brooks</lastName><email>devon@example.com</email></candidate>" +
                "</roster>";

            var candidates = RosterImporter.Parse(xml);

            Assert.AreEqual(2, candidates.Count);
            Assert.AreEqual("Alvarez", candidates[0].LastName);
            Assert.AreEqual("devon@example.com", candidates[1].Email);
        }

        [TestMethod]
        public void Parse_WithEmptyRoster_ReturnsNoCandidates()
        {
            Assert.AreEqual(0, RosterImporter.Parse("<roster />").Count);
        }
    }
}
