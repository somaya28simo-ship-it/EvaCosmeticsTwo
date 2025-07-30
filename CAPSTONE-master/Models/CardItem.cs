using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class CardItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
