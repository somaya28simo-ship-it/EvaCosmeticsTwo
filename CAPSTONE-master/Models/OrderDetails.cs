using Microsoft.EntityFrameworkCore;
using System; 
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public string ProductName { get; set; }   // ✅ أضفناها
        public string ImageUrl { get; set; }      // ✅ أضفناها
        [Precision(18, 2)]
        public decimal Price { get; set; }        // ✅ أضفناها
        [Precision(18, 2)]
        public decimal TotalPrice { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
