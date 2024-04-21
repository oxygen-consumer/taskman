using Microsoft.AspNetCore.Mvc;

namespace TaskmanAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
