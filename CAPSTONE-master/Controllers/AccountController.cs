using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Accounts.FirstOrDefault(a => a.Email == model.Email && a.Password == model.Password);

                if (user != null)
                {
                    HttpContext.Session.SetInt32("AccountId", user.Id);
                    HttpContext.Session.SetString("UserEmail", user.Email);

                    var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, user.Email)
            }, "Login");

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(principal); // ✅ هنا التعديل الصح

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid email or password");
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

       
        public IActionResult MyOrders()
        {
            int? accountId = HttpContext.Session.GetInt32("AccountId");

            if (accountId == null)
            {
                return RedirectToAction("Login");
            }

            var account = _context.Accounts.FirstOrDefault(a => a.Id == accountId);
            if (account == null)
            {
                return RedirectToAction("Login");
            }

            var orders = _context.Orders
                .Where(o => o.AccountId == accountId)
                .Select(o => new MyOrdersViewModel
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalItems = o.OrderDetails.Sum(d => d.Quantity),
                    TotalPrice = o.OrderDetails.Sum(d => d.TotalPrice)
                })
                .ToList();

            var viewModel = new UserOrdersViewModel
            {
                Username = account.Username,
                Email = account.Email,
                Orders = orders
            };

            return View(viewModel);
        }


    }
}
