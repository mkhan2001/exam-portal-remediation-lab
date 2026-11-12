using System.Web.Mvc;
using ExamPortal.Legacy.Web.Services;

namespace ExamPortal.Legacy.Web.Controllers
{
    public class ReportsController : Controller
    {
        public ActionResult Download(string file)
        {
            var reportsFolder = Server.MapPath("~/App_Data/reports");
            var path = ReportFileService.ResolvePath(reportsFolder, file);

            if (!System.IO.File.Exists(path))
            {
                return Content("Report not found: " + path);
            }

            return File(path, ReportFileService.ContentTypeFor(file), file);
        }
    }
}
