using ExamPortal.Legacy.Web.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExamPortal.Legacy.Tests
{
    [TestClass]
    public class SqlQueriesTests
    {
        [TestMethod]
        public void GetCandidateById_PassesIdAsParameter()
        {
            var command = SqlQueries.GetCandidateById(42);

            StringAssert.Contains(command.CommandText, "@id");
            Assert.AreEqual(1, command.Parameters.Count);
            Assert.AreEqual(42, command.Parameters["@id"].Value);
        }

        [TestMethod]
        public void UpdateCandidateNotes_PassesNotesAsParameter()
        {
            var command = SqlQueries.UpdateCandidateNotes(7, "Needs extended time.");

            Assert.AreEqual(2, command.Parameters.Count);
            Assert.AreEqual("Needs extended time.", command.Parameters["@notes"].Value);
            Assert.IsFalse(command.CommandText.Contains("Needs extended time."));
        }

        [TestMethod]
        public void DeleteCandidate_PassesIdAsParameter()
        {
            var command = SqlQueries.DeleteCandidate(3);

            Assert.AreEqual(3, command.Parameters["@id"].Value);
        }
    }
}
