using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using WebApplication1.data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbconnction _context;

        public HomeController(ILogger<HomeController> logger, AppDbconnction context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Fetch New Arrivals (e.g., latest 4 products)
            var newArrivals = _context.Products
                .OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .ToList();

            // Fetch Best Sellers (e.g., products with highest order count)
            var bestSellers = _context.Products
                .Take(4)
               .ToList();

            ViewData["NewArrivals"] = newArrivals;
            ViewData["BestSellers"] = bestSellers;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Chat()
        {
            return View();
        }

        public IActionResult BestSeller()
        {
            return View();
        }

    }
}
