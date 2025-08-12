using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using WebApplication1.data;
using WebApplication1.Models;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ... (كل إعدادات builder services تبقى كما هي) ...
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
            });
            builder.Services.AddDbContext<AppDbconnction>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("J")));
            builder.Services.AddAuthentication("Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });


            var app = builder.Build();

            // ... (كل إعدادات app pipeline تبقى كما هي) ...
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            // Data seeding
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbconnction>();

                // امسحي كل المنتجات القديمة
                context.Products.RemoveRange(context.Products);
                context.SaveChanges();

                // ✅✅✅ أضيفي المنتجات من جديد مع البيانات الكاملة ✅✅✅
                context.Products.AddRange(
                    new Product
                    {
                        Name = "Blossom Glow Kit",
                        Description = "Reveal your skin's natural glow with this nourishing kit.",
                        Price = 320,
                        Stock = 50,
                        ImageUrl = "/images/1vGUmrX0IE2HZrVKmWmJhpZJDpxevTkzYnPIwGv9.webp",
                        Benefits = "Brightens complexion, Evens skin tone, Provides a radiant finish.",
                        HowToUse = "Use the serum first for targeted treatment, followed by the cream to lock in moisture. Use the toner daily for best results.",
                        Ingredients = "Vitamin C, Rosehip Oil, Aloe Vera, Peony Extract."
                    },
                    new Product
                    {
                        Name = "Super Aqua Snail Cream",
                        Description = "Deeply hydrating cream for radiant skin.",
                        Price = 290,
                        Stock = 30,
                        ImageUrl = "/images/super-aqua.webp",
                        Benefits = "Intense hydration, Repairs skin barrier, Improves skin texture.",
                        HowToUse = "Apply a moderate amount to the face and neck as the last step of your skincare routine. Gently pat for better absorption.",
                        Ingredients = "Snail Mucin, Centella Asiatica, Shea Butter."
                    },
                    new Product
                    {
                        Name = "Clarifying Emulsion",
                        Description = "Lightweight emulsion that clears and soothes.",
                        Price = 260,
                        Stock = 25,
                        ImageUrl = "/images/hi8vt2rjk4004pelpsjdvx4ytdmddjhmx7e10lqb.webp",
                        Benefits = "Controls excess sebum, Soothes blemishes, Hydrates without clogging pores.",
                        HowToUse = "After toner, apply an appropriate amount and spread evenly over the face. Can be used in place of a moisturizer for oily skin.",
                        Ingredients = "Tea Tree Oil, Salicylic Acid (BHA), Witch Hazel."
                    },
                    new Product
                    {
                        Name = "Dewy Glow Jelly Cream",
                        Description = "Boost your glow with this jelly-textured moisturizer.",
                        Price = 310,
                        Stock = 40,
                        ImageUrl = "/images/f5rxSDBgEsZNs9sG30dJSyMfNv17fNnoZWMBK3Gv.webp",
                        Benefits = "Provides a dewy, glass-skin finish. Plumps the skin with moisture. Cools and refreshes on contact.",
                        HowToUse = "Apply to face and neck after serum. Can be refrigerated for an extra cooling effect. Perfect as a makeup base.",
                        Ingredients = "Cherry Blossom Extract, Hyaluronic Acid, Glycerin."
                    },
                    new Product
                    {
                        Name = "Lotus Flower Toner",
                        Description = "Hydrating toner infused with lotus flower extracts.",
                        Price = 180,
                        Stock = 45,
                        ImageUrl = "/images/EsAoH9i1as1kPFDZPaS3Hz77BMrp4H4CPCIKSLUw-removebg-preview.png",
                        Benefits = "Deep hydration, Soothes irritated skin, Minimizes pores.",
                        HowToUse = "After cleansing, apply a small amount to a cotton pad and gently wipe over the face. Pat gently for better absorption.",
                        Ingredients = "Lotus Flower Extract, Niacinamide, Hyaluronic Acid."
                    }
                );
                context.SaveChanges();
            }

            app.Run();
        }
    }
}
