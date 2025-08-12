using Microsoft.AspNetCore.Mvc;
using WebApplication1.data; // ✅ 1. أضفنا هذا السطر للوصول إلى قاعدة البيانات
using WebApplication1.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbconnction _context; // ✅ 2. عرفنا متغيرًا لقاعدة البيانات

        // ✅ 3. قمنا بإنشاء Constructor لحقن (inject) خدمة قاعدة البيانات
        public ProductController(AppDbconnction context)
        {
            _context = context;
        }

        // ❌ لقد قمنا بحذف قائمة _allProducts الوهمية بالكامل

        // ✅ 4. تم تعديل هذا الـ Action ليقرأ من قاعدة البيانات الحقيقية
        public IActionResult Index()
        {
            // ببساطة، نقوم بجلب كل المنتجات من جدول Products في قاعدة البيانات
            var products = _context.Products.ToList();

            // نرسل قائمة المنتجات الحقيقية إلى صفحة العرض
            return View(products);
        }

        // ✅ 5. تم تعديل هذا الـ Action ليقرأ من قاعدة البيانات الحقيقية
        public IActionResult Details(int id)
        {
            // ابحث عن المنتج في جدول Products باستخدام الـ id
            var product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound(); // إذا لم يتم العثور على المنتج
            }

            // أرسل المنتج الحقيقي إلى صفحة العرض
            return View(product);
        }
    }
}
