using Microsoft.AspNetCore.Mvc;
using NetworkServicesGateway.Extensions;
using NetworkServicesGateway.Models;

namespace NetworkServicesGateway.Controllers
{
    [Route("/")]
    public class GatewayController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetUserName()))
                return Redirect("/auth");

            return View();
        }
    }
}
