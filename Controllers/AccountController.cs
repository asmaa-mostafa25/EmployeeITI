using EmployeeDep.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDep.Controllers
{
    public class AccountController : Controller
    {
        private const string SessionUserKey = "Username";
        private const string LastLoginCookieKey = "LastLoginDate";

        [HttpGet]
        public IActionResult Login()
        {
            // Bonus #3: show the previous login date (stored last time the user logged in).
            var lastLogin = Request.Cookies[LastLoginCookieKey];
            ViewBag.LastLoginDate = lastLogin;

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionUserKey)))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LastLoginDate = Request.Cookies[LastLoginCookieKey];
                return View(viewModel);
            }

            // Part 3.3: store the username in Session to simulate a logged-in user.
            HttpContext.Session.SetString(SessionUserKey, viewModel.UserName);

            // Bonus #3: remember this login's date/time in a cookie for next time.
            Response.Cookies.Append(
                LastLoginCookieKey,
                DateTime.Now.ToString("f"),
                new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Part 3.4: clear the session and redirect to the Login page.
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
    }
}
