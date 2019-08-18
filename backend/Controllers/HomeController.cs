using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers {
    public class HomeController : Controller {
        [Authorize (AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet ("/")]
        public IActionResult Index () {
            return File ("~/private/index.html", "text/html");
        }

        [HttpGet ("/login")]
        public IActionResult Login () {
            return File ("~/public/index.html", "text/html");
        }

        [HttpGet ("/manifest.json")]
        public IActionResult Manifest () {
            return File ("~/private/manifest.json", "application/json");
        }

        [HttpGet ("/favicon.ico")]
        public IActionResult Favicon () {
            return File ("~/private/favicon.ico", "image/x-icon");
        }

        [Route ("{*url}", Order = 999)]
        public IActionResult Failback () {
            return HttpContext.User.Identity.IsAuthenticated ?
                Index () :
                Login ();
        }
    }
}