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

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbconnction>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("J")));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

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

                // أضيفي المنتجات من جديد
                context.Products.AddRange(
                    new Product
                    {
                        Name = "Blossom Glow Kit",
                        Description = "Reveal your skin's natural glow with this nourishing kit.",
                        Price = 320,
                        Stock = 50,
                        ImageUrl = "/images/cuxziz1odnbfnvxwskehfyojmujq9pv1kfgzlkvh.webp"
                    },
                    new Product
                    {
                        Name = "Super Aqua Snail Cream",
                        Description = "Deeply hydrating cream for radiant skin.",
                        Price = 290,
                        Stock = 30,
                        ImageUrl = "/images/super-aqua.webp"
                    },
                    new Product
                    {
                        Name = "Clarifying Emulsion",
                        Description = "Lightweight emulsion that clears and soothes.",
                        Price = 260,
                        Stock = 25,
                        ImageUrl = "/images/hi8vt2rjk4004pelpsjdvx4ytdmddjhmx7e10lqb.webp"
                    },
                    new Product
                    {
                        Name = "Dewy Glow Jelly Cream",
                        Description = "Boost your glow with this jelly-textured moisturizer.",
                        Price = 310,
                        Stock = 40,
                        ImageUrl = "/images/zmmg2ec33j925tzulqb5tnk9c0g2rka5iipsisxd.webp"
                    },
                    new Product
                    {
                        Name = "Lotus Flower Toner",
                        Description = "Hydrating toner infused with lotus flower extracts.",
                        Price = 180,
                        Stock = 45,
                        ImageUrl = "/images/i0Shk8DgDT3YQ2VJUMvagURvTdSL90AvU3uJKdZL.webp"
                    }
                );
                context.SaveChanges();
            }

            app.Run();

        }
    }
}
