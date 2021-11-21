using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ExamCreator.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("username")))
                return View();
            else
                return Redirect("/Account");
        }
    }
}
