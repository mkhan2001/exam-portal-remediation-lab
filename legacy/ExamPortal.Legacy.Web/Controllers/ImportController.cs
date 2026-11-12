using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using ExamPortal.Legacy.Web.Services;

namespace ExamPortal.Legacy.Web.Controllers
{
    [Authorize]
    public class ImportController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                ViewBag.Message = "Choose a roster file first.";
                return View("Index");
            }

            var savedPath = Path.Combine(Server.MapPath("~/Uploads"), file.FileName);
            Directory.CreateDirectory(Path.GetDirectoryName(savedPath));
            file.SaveAs(savedPath);

            try
            {
                var candidates = RosterImporter.Parse(System.IO.File.ReadAllText(savedPath));
                ViewBag.Message = "Imported " + candidates.Count + " candidates from " + file.FileName + ".";
            }
            catch (Exception)
            {
            }

            return View("Index");
        }
    }
}
