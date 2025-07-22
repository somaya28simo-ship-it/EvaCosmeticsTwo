using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class CardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
