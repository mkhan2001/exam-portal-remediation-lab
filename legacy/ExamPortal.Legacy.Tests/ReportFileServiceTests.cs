using System.IO;
using ExamPortal.Legacy.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExamPortal.Legacy.Tests
{
    [TestClass]
    public class ReportFileServiceTests
    {
        [TestMethod]
        public void ResolvePath_WithSimpleFileName_ReturnsPathInsideReportsFolder()
        {
            var folder = Path.Combine(Path.GetTempPath(), "reports");

            var path = ReportFileService.ResolvePath(folder, "score.pdf");

            Assert.AreEqual(Path.Combine(folder, "score.pdf"), path);
        }

        [TestMethod]
        public void ContentTypeFor_Pdf_ReturnsPdfContentType()
        {
            Assert.AreEqual("application/pdf", ReportFileService.ContentTypeFor("score.PDF"));
        }

        [TestMethod]
        public void ContentTypeFor_UnknownExtension_ReturnsOctetStream()
        {
            Assert.AreEqual("application/octet-stream", ReportFileService.ContentTypeFor("score.xyz"));
        }
    }
}
