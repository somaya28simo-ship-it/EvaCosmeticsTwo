using Microsoft.AspNetCore.Mvc;
using WebApplication1.data;

namespace WebApplication1.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbconnction _context;

        public ProductController(AppDbconnction context)
        {
            _context = context;
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

              return View(product);
        }
    
    }
}
