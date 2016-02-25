using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace Authentication.Cookies.Controllers
{
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/test")]
    public class ApiTestController : Controller
    {
        [HttpGet]
        public IActionResult Info()
        {
            var userInfo = User.Claims.Select(c => new
            {
                c.Issuer,
                c.Type,
                c.Value
            });

            return Ok(userInfo);
        }
    }
}
