using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Helpers;
using WebApplication1.ViewModels;
using WebApplication1.data;

namespace WebApplication1.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbconnction _context;

        public CartController(AppDbconnction context)
        {
            _context = context;
        }

        // ✅ عرض السلة
        public IActionResult Index()
        {
            var cartItems = HttpContext.Session.GetObject<List<CardItem>>("Cart") ?? new List<CardItem>();
            return View(cartItems);
        }

        // ✅ إضافة منتج للسلة
        public IActionResult Add(int productId, string productName, string imageUrl, decimal price)
        {
            var cartItems = HttpContext.Session.GetObject<List<CardItem>>("Cart") ?? new List<CardItem>();

            var existingItem = cartItems.FirstOrDefault(c => c.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cartItems.Add(new CardItem
                {
                    ProductId = productId,
                    ProductName = productName,
                    ImageUrl = imageUrl,
                    Price = price,
                    Quantity = 1
                });
            }

            HttpContext.Session.SetObject("Cart", cartItems);
            return RedirectToAction("Index");
        }

        // ✅ إزالة منتج من السلة
        public IActionResult Remove(int id)
        {
            var cartItems = HttpContext.Session.GetObject<List<CardItem>>("Cart") ?? new List<CardItem>();

            var item = cartItems.FirstOrDefault(c => c.ProductId == id);
            if (item != null)
            {
                cartItems.Remove(item);
            }

            HttpContext.Session.SetObject("Cart", cartItems);
            return RedirectToAction("Index");
        }

        // ✅ عرض صفحة الدفع
        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }

        // ✅ تنفيذ الطلب وحفظه في قاعدة البيانات
        [HttpPost]
        public IActionResult Checkout(CheckoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                var cartItems = HttpContext.Session.GetObject<List<CardItem>>("Cart") ?? new List<CardItem>();

                var firstAccount = _context.Accounts.FirstOrDefault();
                if (firstAccount == null)
                {
                    ModelState.AddModelError("", "No user account found. Please register first.");
                    return View(model);
                }

                var order = new Order
                {
                    CustomerName = model.CustomerName,
                    Address = model.Address,
                    OrderDate = DateTime.Now,
                    AccountId = firstAccount.Id,
                    OrderDetails = new List<OrderDetails>()
                };

                foreach (var item in cartItems)
                {
                    order.OrderDetails.Add(new OrderDetails
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        ImageUrl = item.ImageUrl,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        TotalPrice = item.Price * item.Quantity
                    });
                }

                _context.Orders.Add(order);
                _context.SaveChanges();

                HttpContext.Session.Remove("Cart"); // 🧼 تفريغ السيشن

                return RedirectToAction("OrderConfirmation");
            }

            return View(model);
        }

        // ✅ صفحة تأكيد الطلب
        public IActionResult OrderConfirmation()
        {
            return View();
        }
        public IActionResult MyOrders()
        {
            var account = _context.Accounts.FirstOrDefault(); // ممكن تربطيها بالـ session لاحقًا
            if (account == null)
                return RedirectToAction("Login", "Account");

            var orders = _context.Orders
                .Where(o => o.AccountId == account.Id)
                .Select(o => new MyOrdersViewModel
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalItems = o.OrderDetails.Sum(d => d.Quantity),
                    TotalPrice = o.OrderDetails.Sum(d => d.TotalPrice)
                })
                .ToList();

            return View(orders);
        }

    }
}