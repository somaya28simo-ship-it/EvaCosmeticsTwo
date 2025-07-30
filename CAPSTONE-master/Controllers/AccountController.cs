using Microsoft.AspNetCore.Mvc;
using WebApplication1.data;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbconnction _context;

        public AccountController(AppDbconnction context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Accounts
                    .FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserRole", user.Role);
                    HttpContext.Session.SetString("Username", user.Username);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid email or password.");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // هنا بنحفظ بيانات المستخدم في قاعدة البيانات
                var account = new Account
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    Role = "User"
                };

                _context.Accounts.Add(account);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(model);
        }
        public IActionResult MyOrders()
        {
            var user = _context.Accounts.FirstOrDefault(); // مؤقتًا أول مستخدم

            if (user == null)
                return RedirectToAction("Login");

            var orders = _context.Orders
                .Where(o => o.AccountId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

    }
}
