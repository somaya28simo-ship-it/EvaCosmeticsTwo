using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Helpers;
using WebApplication1.ViewModels;
using WebApplication1.data;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    [Authorize]
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

        // ✅ إضافة منتج للسلة (النسخة المعدلة والصحيحة)
        public IActionResult Add(int productId) //  1️⃣ استقبلنا الـ ID فقط
        {
            // 2️⃣ ابحث عن المنتج كاملاً في قاعدة البيانات
            var productToAdd = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (productToAdd == null)
            {
                // إذا لم يتم العثور على المنتج، لا تفعل شيئًا أو أرجع خطأ
                return RedirectToAction("Index", "Home"); // العودة للصفحة الرئيسية
            }

            // 3️⃣ أحضر السلة من الذاكرة (Session)
            var cartItems = HttpContext.Session.GetObject<List<CardItem>>("Cart") ?? new List<CardItem>();

            var existingItem = cartItems.FirstOrDefault(c => c.ProductId == productId);
            if (existingItem != null)
            {
                // إذا كان المنتج موجودًا، زد الكمية فقط
                existingItem.Quantity++;
            }
            else
            {
                // إذا لم يكن موجودًا، أنشئ عنصرًا جديدًا بالبيانات الحقيقية من قاعدة البيانات
                cartItems.Add(new CardItem
                {
                    ProductId = productToAdd.Id,
                    ProductName = productToAdd.Name,
                    ImageUrl = productToAdd.ImageUrl,
                    Price = productToAdd.Price, //  4️⃣ ✅ هذا هو السطر الأهم، يأخذ السعر الحقيقي
                    Quantity = 1
                });
            }

            // 5️⃣ احفظ السلة المحدثة في الذاكرة
            HttpContext.Session.SetObject("Cart", cartItems);

            // 6️⃣ أعد توجيه المستخدم إلى صفحة سلة التسوق
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

        [HttpPost]
        public IActionResult Checkout(CheckoutViewModel model)
        {
            // 1. التحقق من تسجيل الدخول
            int? accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. التحقق من صحة النموذج (Model)
            if (ModelState.IsValid)
            {
                // 3. الحصول على عناصر سلة التسوق من الجلسة (Session)
                var cartItems = HttpContext.Session.GetObject<List<CardItem>>("Cart") ?? new List<CardItem>();

                if (cartItems.Count == 0)
                {
                    // لا تسمح بإنشاء طلب فارغ
                    ModelState.AddModelError("", "Your cart is empty.");
                    return View(model); // أعده إلى صفحة الدفع مع رسالة خطأ
                }

                // 4. إنشاء الطلب وربطه بالمستخدم الحالي
                var order = new Order
                {
                    CustomerName = model.CustomerName,
                    Address = model.Address,
                    OrderDate = DateTime.Now,
                    AccountId = accountId.Value,
                    OrderDetails = new List<OrderDetails>()
                };

                // 5. إضافة تفاصيل الطلب من سلة التسوق
                foreach (var item in cartItems)
                {
                    order.OrderDetails.Add(new OrderDetails
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        TotalPrice = item.Price * item.Quantity,
                        // ملاحظة: لا داعي لإضافة الاسم والصورة والسعر هنا إذا كنت ستصل إليها عبر Product
                          ProductName = item.ProductName,
                        ImageUrl = item.ImageUrl,
                        Price = item.Price
                    });
                }

                // 6. حفظ الطلب في قاعدة البيانات
                _context.Orders.Add(order);
                _context.SaveChanges();

                // 7. تفريغ سلة التسوق بعد إتمام الطلب
                HttpContext.Session.Remove("Cart");

                // 8. إعادة توجيه المستخدم إلى صفحة تأكيد الطلب
                return RedirectToAction("OrderConfirmation"); // ✅ إرجاع قيمة في المسار الصحيح
            }

            // 9. إذا كان النموذج غير صالح، أعد عرض الصفحة مع الأخطاء
            return View(model); // ✅ إرجاع قيمة في حالة فشل التحقق
        }

        // أضف هاتين الدالتين الجديدتين داخل CartController

        // ✅ لزيادة الكمية
        [HttpPost]
        public IActionResult IncreaseQuantity(int productId)
        {
            var cartItems = HttpContext.Session.GetObject<List<CardItem>>("Cart") ?? new List<CardItem>();
            var item = cartItems.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                item.Quantity++; // زد الكمية بواحد
            }

            HttpContext.Session.SetObject("Cart", cartItems);
            return RedirectToAction("Index");
        }

        // ✅ لتقليل الكمية
        [HttpPost]
        public IActionResult DecreaseQuantity(int productId)
        {
            var cartItems = HttpContext.Session.GetObject<List<CardItem>>("Cart") ?? new List<CardItem>();
            var item = cartItems.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--; // قلل الكمية بواحد
                }
                else
                {
                    // إذا كانت الكمية 1 وضغط على ناقص، قم بإزالة المنتج
                    cartItems.Remove(item);
                }
            }

            HttpContext.Session.SetObject("Cart", cartItems);
            return RedirectToAction("Index");
        }

        // ✅ صفحة تأكيد الطلب
        public IActionResult OrderConfirmation()
        {
            return View();
        }
       

    }
}