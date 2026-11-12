using System.IO;

namespace ExamPortal.Legacy.Web.Services
{
    /// <summary>Locates score report files that candidates can download.</summary>
    public static class ReportFileService
    {
        public static string ResolvePath(string reportsFolder, string fileName)
        {
            return Path.Combine(reportsFolder, fileName);
        }

        public static string ContentTypeFor(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            switch (extension)
            {
                case ".pdf": return "application/pdf";
                case ".csv": return "text/csv";
                case ".txt": return "text/plain";
                default: return "application/octet-stream";
            }
        }
    }
}
