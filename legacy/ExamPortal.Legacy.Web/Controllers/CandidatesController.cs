using System.Web;
using System.Web.Mvc;
using ExamPortal.Legacy.Web.Data;
using ExamPortal.Legacy.Web.Services;
using Newtonsoft.Json;

namespace ExamPortal.Legacy.Web.Controllers
{
    [Authorize]
    public class CandidatesController : Controller
    {
        private readonly CandidateRepository _candidates = new CandidateRepository();

        [ValidateInput(false)]
        public ActionResult Index(string search, string email)
        {
            var prefsCookie = Request.Cookies[PreferencesCookie.CookieName];
            ViewBag.Preferences = PreferencesCookie.Deserialize(prefsCookie == null ? null : prefsCookie.Value);
            ViewBag.Search = search;

            var results = string.IsNullOrEmpty(email)
                ? _candidates.SearchByLastName(search)
                : _candidates.SearchByEmail(email);

            return View(results);
        }

        public ActionResult Details(int id)
        {
            var candidate = _candidates.GetById(id);
            if (candidate == null)
            {
                return HttpNotFound();
            }

            ViewBag.ReportAccessCode = CryptoHelper.Encrypt(candidate.Id + "|" + candidate.Email);
            return View(candidate);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateNotes(int id, string notes)
        {
            _candidates.UpdateNotes(id, notes);
            return RedirectToAction("Details", new { id });
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            _candidates.Delete(id);
            return RedirectToAction("Index");
        }

        public ActionResult Export(string search)
        {
            var results = _candidates.SearchByLastName(search);
            return Content(JsonConvert.SerializeObject(results), "application/json");
        }

        [HttpPost]
        public ActionResult SavePreferences(string theme, int pageSize)
        {
            var preferences = new Models.UserPreferences { Theme = theme, PageSize = pageSize };
            Response.Cookies.Add(new HttpCookie(PreferencesCookie.CookieName, PreferencesCookie.Serialize(preferences)));
            return RedirectToAction("Index");
        }
    }
}
