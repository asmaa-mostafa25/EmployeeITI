using EmployeeDep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeeDep.Controllers
{
    public class HomeController : Controller
    {
        private const string UserNameCookieKey = "UserName";
        private const string ThemeCookieKey = "Theme";
        private const string VisitCountSessionKey = "HomeVisitCount";

        public IActionResult Index()
        {
            // Part 2.2: Welcome message based on the "UserName" cookie.
            var userName = Request.Cookies[UserNameCookieKey];
            ViewBag.WelcomeMessage = string.IsNullOrEmpty(userName)
                ? "Welcome, Guest"
                : $"Welcome, {userName}";

            // Part 3.1: visit counter using Session, incremented on every refresh.
            int visitCount = HttpContext.Session.GetInt32(VisitCountSessionKey) ?? 0;
            visitCount++;
            HttpContext.Session.SetInt32(VisitCountSessionKey, visitCount);
            ViewBag.VisitCount = visitCount;

            return View();
        }

        [HttpGet]
        public IActionResult SetUserName()
        {
            var model = new SetUserNameViewModel
            {
                UserName = Request.Cookies[UserNameCookieKey]
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult SetUserName(SetUserNameViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Part 2.1: store the username inside a Cookie.
            Response.Cookies.Append(
                UserNameCookieKey,
                viewModel.UserName,
                new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult SetTheme(string theme)
        {
            // Part 2.3: save the selected theme (Light/Dark) inside a Cookie.
            if (theme == "Dark" || theme == "Light")
            {
                Response.Cookies.Append(
                    ThemeCookieKey,
                    theme,
                    new CookieOptions { Expires = DateTimeOffset.Now.AddDays(365) });
            }

            var referer = Request.Headers.Referer.ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new EmployeeDep.Models.ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
