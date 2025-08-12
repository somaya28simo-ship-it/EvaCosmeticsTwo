using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.data;
using WebApplication1.Models;
using WebApplication1.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
  
    public class AccountController : Controller
    {
        private readonly AppDbconnction _context;

        public AccountController(AppDbconnction context)
        {
            _context = context;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = _context.Accounts.FirstOrDefault(a => a.Email == model.Email && a.Password == model.Password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("AccountId", user.Id);
                HttpContext.Session.SetString("UserEmail", user.Email);

                var identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, user.Email)
                    }, "Cookies");

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("Cookies", principal);


                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid email or password");

            return View(model);
        }



        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Accounts.FirstOrDefault(a => a.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(model);
                }

                var newAccount = new Account
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password, // ❗ في المستقبل، استخدمي تشفير
                    Role = "User"
                };

                _context.Accounts.Add(newAccount);
                _context.SaveChanges();

                // تسجيل الجلسة والدخول بعد التسجيل
                HttpContext.Session.SetInt32("AccountId", newAccount.Id);
                HttpContext.Session.SetString("UserEmail", newAccount.Email);

                var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, newAccount.Email)
                }, "Cookies");

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("Cookies", principal);


                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [Authorize]
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
