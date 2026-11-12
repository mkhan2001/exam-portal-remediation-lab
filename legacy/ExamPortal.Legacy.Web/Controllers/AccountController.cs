using System.Web.Mvc;
using System.Web.Security;
using ExamPortal.Legacy.Web.Data;
using ExamPortal.Legacy.Web.Models;
using ExamPortal.Legacy.Web.Services;

namespace ExamPortal.Legacy.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _users = new UserRepository();

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginModel());
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            var user = _users.FindByUsername(model.Username);
            if (user == null)
            {
                ModelState.AddModelError("", "No account exists for the username '" + model.Username + "'.");
                return View(model);
            }

            if (!PasswordHasher.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "The password is incorrect.");
                return View(model);
            }

            Session["UserId"] = user.Id;
            Session["Username"] = user.Username;
            Session["Role"] = user.Role;
            FormsAuthentication.SetAuthCookie(user.Username, true);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Candidates");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
