using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending";

        public string CustomerName { get; set; }
        public string Address { get; set; }

        [ForeignKey("AccountId")]
        public int AccountId { get; set; }
        public Account Account { get; set; }

        public List<OrderDetails> OrderDetails { get; set; }
    }
}
