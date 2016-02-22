using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace Authentication.Cookies.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(ActiveAuthenticationSchemes = "Cookies")]
        public IActionResult Index()
        {
            return View();
        }

        //[Authorize]
        [Authorize(ActiveAuthenticationSchemes = "Cookies")]
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
