using Microsoft.AspNetCore.Mvc;
using NetworkServicesGateway.Extensions;
using NetworkServicesGateway.Models;

namespace NetworkServicesGateway.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet("/auth")]
        public IActionResult Authorize()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetUserName()))
                return Redirect("/");

            return View("Login");
        }

        [HttpPost("/auth")]
        public IActionResult Authorize(User userModel)
        {
            if (ModelState.IsValid && userModel.Name == "admin" && userModel.Password == "admin")
            {
                HttpContext.Session.SetUserName(userModel.Name);
                return Redirect("/");
            }
            ModelState.AddModelError(nameof(userModel.Name), "Błędny login lub hasło");
            return View("Login", userModel);
        }
    }
}
