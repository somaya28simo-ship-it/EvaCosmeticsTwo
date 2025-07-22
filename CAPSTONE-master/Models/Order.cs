using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Order
    {
         public int Id { get; set; }

        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = "Pending";
        [ForeignKey("AccountId")]
        public int AccountId { get; set; }
        public  Account Account{ get; set; }
    }
}
