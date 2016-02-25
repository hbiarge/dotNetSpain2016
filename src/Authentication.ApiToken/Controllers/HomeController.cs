using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace Authentication.Cookies.Controllers
{
    [Authorize(ActiveAuthenticationSchemes = "Cookies")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Secure()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
