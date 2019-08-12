using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers {
    public class HomeController : Controller {
        [Authorize]
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
    }
}